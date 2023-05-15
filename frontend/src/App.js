import styles from './css/app.module.css';
import {useDispatch, useSelector} from "react-redux";
import {setUserData} from "./context/store";
import {Route, Routes} from "react-router-dom";
import Header from "./components/layout/header/header";
import Footer from "./components/layout/footer";
import {AuthService} from "./services/authService";
import {useEffect} from "react";
import {Profile} from "./components/profile/profile";
import {GamesPage} from "./components/gamesPage/gamesPage";
import {ErrorPage} from "./components/responses/errorPage";

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

            <Route path={'games'} element={<GamesPage />}/>

            <Route path={'/'} element={<GamesPage />} />
            <Route path={'*'} element={<ErrorPage code={404}/>} />
          </Routes>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default App;
