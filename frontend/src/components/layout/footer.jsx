import styles from '../../css/app.module.css'

const Footer = () => {
    return(
        <footer>
            <span className={styles.footerText}>
                All rights reserved! See on <a className={styles.white} href="https://github.com/R0ckel/GameShop">GitHub</a>
            </span>
        </footer>
    )
}

export default Footer