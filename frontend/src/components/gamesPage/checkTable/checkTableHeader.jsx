import React from 'react';
import styles from "../../../css/app.module.css"

export default function CheckTableHeader({template, cardViewFields, withImage = false}) {
	const tableCells = [];
	if (withImage) tableCells.push("Thumbnail")
	for (const key in template) {
		if (Object.hasOwnProperty.call(template, key)
			&& cardViewFields.includes(key)
		) {
			tableCells.push(key);
		}
	}
	tableCells.push("Checked");
	tableCells.push("Action");
	if (template) {
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