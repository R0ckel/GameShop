import {statusCodeToMessage} from '../../variables/statusCodeToMessage'
import styles from "../../css/app.module.css";

export const ErrorPage = ({code}) => {
	const message = statusCodeToMessage[code];

	return(
		<div className={styles.centeredInfoBlock} style={{color: "orange"}}>
			<h1>{code} {message?.title ?? "Unexpected error"}</h1>
			<h2>{message?.description}</h2>
		</div>
	)
}