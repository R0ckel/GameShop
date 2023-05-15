import React from 'react';
import styles from "../../../css/app.module.css"

export function CheckListHeader({shown, selected}) {
	return (
		<div className={`${styles.listHeader}`}>
			<span>Shown: {shown}</span>
			<span>Selected: {selected}</span>
		</div>
	);
}