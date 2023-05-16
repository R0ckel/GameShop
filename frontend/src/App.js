import styles from './css/app.module.css';
import {useDispatch, useSelector} from "react-redux";
import {setUserData} from "./context/store";
import {Navigate, Route, Routes} from "react-router-dom";
import Header from "./components/layout/header/header";
import Footer from "./components/layout/footer";
import {AuthService} from "./services/authService";
import {useEffect} from "react";
import {Profile} from "./components/profile/profile";
import {GamesPage} from "./components/gamesPage/gamesPage";
import {ErrorPage} from "./components/responses/errorPage";
import GameDetails from "./components/detailsPage/gameDetails";
import {AdminGamePanel} from "./components/admin/adminGamePanel";
import {AdminCompanyPanel} from "./components/admin/adminCompanyPanel";
import {AdminGameGenrePanel} from "./components/admin/adminGameGenrePanel";

const App = () => {
  const dispatch = useDispatch()
  const {role} = useSelector(state => state.userData);

  useEffect( () => {
    async function fetchData() {
      if (role == null || role.toString().trim().length === 0) {
        const userClaims = await AuthService.getUserClaims();
        if (userClaims != null) {
          await AuthService.applyUserDataToContext(userClaims, dispatch)
          return;
        }
        dispatch(setUserData({isLoggedIn: false}))
      }
    }

    fetchData();
  }, [dispatch, role]);

  return (
    <div className={styles.app}>
      <div className={styles.wrapper}>
        <Header key={'header'}/>
        <div className={styles.pageContent}>
          <Routes>
            <Route path={'profile/:id'} element={<Profile />}/> {/* profile by id */}
            <Route path={'profile'} element={<Profile />}/> {/* current profile */}

            <Route path={'games/:id'} element={<GameDetails />}/> {/* game details */}
            <Route path={'games'} element={<GamesPage />}/> {/* main page with games */}

            <Route path={'admin/games'} element={<AdminGamePanel /> }/>
            <Route path={'admin/companies'} element={<AdminCompanyPanel /> }/>
            <Route path={'admin/gameGenres'} element={<AdminGameGenrePanel /> }/>

            <Route path={'/'} element={<Navigate to={'games'}/>} />
            <Route path={'*'} element={<ErrorPage code={404}/>} />
          </Routes>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default App;
