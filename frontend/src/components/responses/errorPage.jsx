import {statusCodeToMessage} from '../../variables/statusCodeToMessage'
import styles from "../../css/app.module.css";
import {Link} from "react-router-dom";
import { useState, useEffect } from 'react';

export const ErrorPage = ({code}) => {
	const [showError, setShowError] = useState(false);
	const message = statusCodeToMessage[code];

	useEffect(() => {
		const timer = setTimeout(() => setShowError(true), 300);
		return () => clearTimeout(timer);
	}, []);

	if (!showError) return null;

	return (
		<div className={styles.centeredInfoBlock} style={{color: "orange"}}>
			<h1>{code} {message?.title ?? "Unexpected error"}</h1>
			<h2>{message?.description}</h2>
			<Link to={'/'}>
				<button className={`${styles.btn} ${styles.noLink}`}>
					Return to main
				</button>
			</Link>
		</div>
	);
}