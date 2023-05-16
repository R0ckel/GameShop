import {statusCodeToMessage} from '../../variables/statusCodeToMessage'
import styles from "../../css/app.module.css";
import {Link} from "react-router-dom";

export const ErrorPage = ({code}) => {
	const message = statusCodeToMessage[code];

	setTimeout(()=>{
		return(
			<div className={styles.centeredInfoBlock} style={{color: "orange"}}>
				<h1>{code} {message?.title ?? "Unexpected error"}</h1>
				<h2>{message?.description}</h2>
				<Link to={'/'}>
					<button className={`${styles.btn} ${styles.noLink}`}>
						Return to main
					</button>
				</Link>
			</div>
		)
	}, 300)
}