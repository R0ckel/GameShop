import styles from "../../css/app.module.css";
import Image from "../helpers/image";
import React from "react";
import defaultUserAvatar from '../../image/user-icon.png';
import {Link} from "react-router-dom";
import {userImagesApiUrl} from "../../variables/connectionVariables";
import {useSelector} from "react-redux";
import {Modal} from "antd";
import {CommentService} from "../../services/domain/commentService";
import {DeleteOutlined} from "@ant-design/icons";

export const Comment = ({comment, updateCommentList}) => {
	const {userId} = useSelector(state => state.userData)

	function deleteComment(){
		Modal.confirm({
			title: 'Are you sure you want to delete this comment?',
			onOk: async () => {
				const response = await CommentService.delete(comment.id);
				if (response?.success) {
					updateCommentList()
				}
			},
		});
	}

	const getRandomColor = () => {
		const r = Math.floor(Math.random() * 256);
		const g = Math.floor(Math.random() * 256);
		const b = Math.floor(Math.random() * 256);
		const a = Math.random() * (0.3 - 0.05) + 0.05;
		return `rgba(${r}, ${g}, ${b}, ${a})`;
	}

	const randomBackgroundColor = {
		backgroundColor: getRandomColor(),
		border: '1px solid white'
	}

	return(
		<div style={randomBackgroundColor} className={`${styles.comment}`}>
			<div className={styles.commentSidebar}>
				<Link to={`/profile/${comment?.userId}`} className={styles.noLink}>
					<Image
						src={`${userImagesApiUrl}/${comment?.userId}?thumbnail=true`}
						defaultImage={defaultUserAvatar}
						imageClassName={`${styles.thumbnail} ${styles.smoothBorder}`}
						containerClassName={`${styles.centered}`}
					/>
					<h4>{comment?.userName}</h4>
				</Link>
				<span>{new Date(comment?.created).toLocaleDateString()}</span>
				<span>{new Date(comment?.created).toLocaleTimeString()}</span>
			</div>
			<div style={randomBackgroundColor} className={`${styles.smoothBorder} ${styles.commentTextBlock}`}>
				{comment?.text}
			</div>
			{userId === comment.userId ?
				<div>
					<DeleteOutlined danger='true' onClick={deleteComment}>
						Delete
					</DeleteOutlined>
				</div>
				: <></>}

		</div>
	)
}