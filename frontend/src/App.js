import styles from './css/app.module.css';
import {useDispatch, useSelector} from "react-redux";
import {setCardViewFields, setUserData, setProducts} from "./context/store";
import {Route, Routes} from "react-router-dom";
import Header from "./components/layout/header/header";
import Footer from "./components/layout/footer";
import {authService} from "./services/authService";
import {useEffect} from "react";
import {Profile} from "./components/profile/profile";

const allItems = [];

const App = () => {
  const dispatch = useDispatch()
  const {role} = useSelector(state => state.userData);

  useEffect( () => {
    async function fetchData() {
      if (role == null || role.toString().trim().length === 0) {
        const userClaims = await authService.getUserClaims();
        if (userClaims != null) {
          await authService.applyUserDataToContext(userClaims, dispatch)
          return;
        }
        dispatch(setUserData({isLoggedIn: false}))
      }
    }

    fetchData();
  }, [dispatch, role]);

  useEffect(() => {
    dispatch(setCardViewFields(['mark', 'model', 'price']))
    dispatch(setProducts(allItems.map(item => {
      return {
        ...item,
        selected: false
      };
    })))
  }, [dispatch]);

  return (
    <div className={styles.app}>
      <div className={styles.wrapper}>
        <Header key={'header'}/>
        <div className={styles.pageContent}>
          <Routes>
            <Route path={'profile/:id'} element={<Profile/>}/>
            <Route path={'profile'} element={<Profile/>}/>


            {/*<Route path={`productCategories/:categoryName`} element={<ProductsPageWrapper/>}/>*/}
            {/*<Route path={`productCategories`} element={<ProductsPageWrapper/>}/>*/}

            {/*<Route path={`admin/products`} element={<AdminProductsPage/>}/>*/}

            {/*<Route path={'products/:id'} element={<ProductDetails/>}/>*/}
            {/*<Route path={"*"} element={<Navigate to={'/productCategories'}/>}/>*/}
            <Route path={'/'} element={<></>} />
          </Routes>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default App;
