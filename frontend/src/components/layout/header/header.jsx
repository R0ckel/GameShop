import React from 'react';
import LoginPanel from "./loginPanel";
import Image from "../../helpers/image";
import logo from "../../../image/logo192.png";
import styles from "../../../css/app.module.css";
import {Link} from "react-router-dom";
import {useSelector} from "react-redux";
import {Basket} from "./basket";

const Header = () => {
  const {role, isLoggedIn} = useSelector(state => state.userData);

  return (
    <header>
      <Image containerClassName={styles.headerLogo} src={logo}/>
      <Link to={`/`} className={`${styles.noLink}`}>
        <h2 className={`${styles.app}`}> Rockel Game Shop </h2>
      </Link>

      {
        role.toLowerCase() === 'admin' ?
          <Link to={`/admin/products`} className={`${styles.noLink} ${styles.headerButton} ${styles.aleft}`}>
            <span className={`${styles.app}`}> Admin Panel </span>
          </Link> :
          <></>
      }

      <div className={`${styles.headerButton} ${styles.aright}`} style={{
        display: 'flex',
        justifyContent: "space-evenly",
        alignContent: "center"
      }}>
        {
          isLoggedIn ? <Basket /> : <></>
        }
        <LoginPanel/>
      </div>
    </header>
  );
}

export default Header