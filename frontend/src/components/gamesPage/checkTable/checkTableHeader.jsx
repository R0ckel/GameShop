import React from 'react';
import styles from "../../../css/app.module.css"
import {useSelector} from "react-redux";

export default function CheckTableHeader({template, cardViewFields, withImage = false}) {
	const {isLoggedIn} = useSelector(state => state.userData)

	if (template) {
		const tableCells = [];
		if (withImage) tableCells.push("Thumbnail");
		for (const key of cardViewFields) {
			if (Object.hasOwnProperty.call(template, key)) {
				tableCells.push(key);
			}
		}
		if (isLoggedIn) tableCells.push("Checked");
		tableCells.push("Action");

		return (
			<thead className={`${styles.capitalize}`}>
			<tr>
				{tableCells.map(tableCell => (
					<th key={template.id + tableCell}>{tableCell}</th>
				))}
			</tr>
			</thead>
		)
	}
	return <></>
}