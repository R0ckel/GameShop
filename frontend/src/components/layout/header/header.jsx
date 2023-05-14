import React from 'react';
import LoginPanel from "./loginPanel";
import Image from "../../helpers/image";
import logo from "../../../image/logo192.png";
import styles from "../../../css/app.module.css";
import {Link} from "react-router-dom";
import {Button} from "antd";
import {basketService} from "../../../services/basketService";
import {useSelector} from "react-redux";

const Header = () => {
  const {role} = useSelector(state => state.userData);

  return (
    <header>
      <Image containerClassName={styles.headerLogo} src={logo}/>
      <Link to={`/`} className={`${styles.noLink}`}>
        <h2 className={`${styles.app}`}> MyReactApp </h2>
      </Link>

      {
        role === 'admin' ?
          <Link to={`/admin/products`} className={`${styles.noLink} ${styles.headerButton} ${styles.aleft}`}>
            <span className={`${styles.app}`}> Admin Panel </span>
          </Link> :
          <></>
      }

      <Button onClick={async () => console.log(await basketService.getBasket(localStorage.getItem('token')))}>
        Get Basket
      </Button>

      <div className={`${styles.headerButton} ${styles.aright}`}>
        <LoginPanel/>
      </div>
    </header>
  );
}

export default Header