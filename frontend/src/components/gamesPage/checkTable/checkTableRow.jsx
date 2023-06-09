import React, {useState} from 'react';
import {Link} from "react-router-dom";
import styles from "../../../css/app.module.css";
import checkboxStyle from "../../../css/checkbox.module.css"
import {useSelector} from "react-redux";
import {message} from "antd";
import Image from "../../helpers/image";

const CheckTableRow = ({item, cardViewFields, updateSender, isChecked, thumbnailSrc, defaultThumbnailSrc}) => {
	const {isLoggedIn} = useSelector(state => state.userData)
	const [checked, setCheck] = useState(isChecked);

	function toggleCheck() {
		if (updateSender != null) updateSender(item.id, !checked);
		setCheck(!checked);
	}

	const cells = cardViewFields.map(field => {
		const val = item[field];
		return val ? (
			<td key={`${item.id}${field}${val}`}>{field === 'price'? `${val}$`: val}</td>
		) : null;
	});

	const handleCheck = () => {
		if (!isLoggedIn) {
			message.warning("You must be authorized to check products!");
			return;
		}
		toggleCheck();
	};

	return (
		<tr style={checked ? {backgroundColor: 'rgba(0,2,51,0.55)'}: {}}>
			{thumbnailSrc == null ? <></> :
				<td>
					<Image
						src={thumbnailSrc}
						defaultImage={defaultThumbnailSrc}
						imageClassName={`${styles.thumbnail} ${styles.smoothBorder}`}
					/>
				</td>
			}
			{cells}
			{isLoggedIn ?
				<td>
					<div className={checkboxStyle["checkbox-wrapper"]}>
						<input
							type="checkbox"
							id={item.id + "_checkbox"}
							defaultChecked={checked}
							disabled={!isLoggedIn}
						/>
						<label htmlFor={item.id + "_checkbox"}
						       className={checkboxStyle["check-box"]}
						       onClick={handleCheck}>
						</label>
					</div>
				</td>
				: null
			}
			<td>
				<Link to={`/games/${item.id}`}>
					<button className={`${styles.btn} ${styles.info}`}>
						Details
					</button>
				</Link>
			</td>
		</tr>
	);
};

export default CheckTableRow;