import React, {useEffect, useRef, useState} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useParams} from 'react-router-dom';
import { ProfileService } from '../../services/profileService';
import { KeyValueTable } from '../helpers/keyValueTable';
import styles from '../../css/app.module.css';
import Image from "../helpers/image";
import {userImagesApiUrl} from "../../variables/connectionVariables";
import {ErrorPage} from "../responses/errorPage";
import {Button, Modal, Upload, Input, message} from "antd";
import {UploadOutlined} from '@ant-design/icons';
import * as Yup from "yup";
import {Field, Form, Formik} from "formik";
import styled from "styled-components";
import {AuthService} from "../../services/authService";
import defaultUserIcon from "../../image/user-icon.png"

const EditProfileSchema = Yup.object().shape({
	name: Yup.string()
	.max(40, 'First name must be at most 40 characters long'),
	surname: Yup.string()
	.max(40, 'Last name must be at most 40 characters long'),
	email: Yup.string()
	.email('Invalid email address'),
	birthDate: Yup.date(),
	currentPassword: Yup.string().required('Please enter your current password!'),
	newPassword: Yup.string()
	.matches(
		/^(?=.*[A-Z])(?=.*[^a-zA-Z])/,
		'Password must contain at least one uppercase letter and one non-letter character'
	)
	.matches(/\d/, 'Password must contain at least one digit')
	.min(8, 'Password must be at least 8 characters long')
	.max(64, 'Password must be at most 64 characters long')
	.notRequired(),

	confirmPassword: Yup.string()
	.oneOf([Yup.ref('newPassword')], 'Passwords must match')
	.notRequired(),
});

const MarginedLabel = styled("label")`
  margin: 10px 0 2px;
`;

const MarginedButton = styled(Button)`
  margin-top: 20px;
`;

const Error = styled("div")`
  color: red;
`;

export const Profile = () => {
	const [userData, setUserData] = useState({});
	const { userId, email } = useSelector(state => state.userData);
	const { id } = useParams();
	const finalId = id || userId;
	const formikRef = useRef();
	const dispatch = useDispatch();

	//for image upload
	const [fileList, setFileList] = useState([]);
	const [isEditAvatarModalOpen, setIsEditAvatarModelOpen] = useState(false);
	const [avatarUrl, setAvatarUrl] = useState(`${userImagesApiUrl}/${finalId}?${Date.now()}`);
	const [isEditUserModalOpen, setIsEditUserModalOpen] = useState(false);
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [lastUpdateTime, setLastUpdateTime] = useState(Date.now())
	const [userValuesToShow, setUserValuesToShow] = useState(userData)

	useEffect(()=>{
		setAvatarUrl(`${userImagesApiUrl}/${finalId}?${Date.now()}`)
	}, [finalId])

	useEffect(() => {
		const fetchData = async () => {
			const data = await ProfileService.getUser(id || userId);
			setUserData(data);
		}
		if (id || userId) {
			fetchData();
		}
	}, [id, userId, lastUpdateTime]);

	useEffect(() => {
		updateUDWID(userData)
	}, [userData])

	if (finalId === null || finalId.trim().length === 0){
		return(
			<ErrorPage code={401}/>
		)
	}

	function updateUDWID(data){
		// Create a new object that doesn't include the 'id' property
		setUserValuesToShow(Object.fromEntries(
			Object.entries(data?.values?.[0] ?? {}).filter(([key]) => key !== 'id')
		));
	}

	const updateTime = () => {
		console.log('update')
		setLastUpdateTime(Date.now())
	}

	const handleEditUserSubmit = async (values) => {
		setIsSubmitting(true);
		try{
			const response = await ProfileService.updateUser({ ...values, id: finalId });
			if (response?.success) {
				setIsEditUserModalOpen(false);
				message.success(response.message)

				// re-login to update data
				const loginEmail = values.email.length > 0 ? values.email : email;
				const loginPassword = values.newPassword.length > 0 ? values.newPassword : values.currentPassword;
				const loginResponse = await AuthService.login({email: loginEmail, password: loginPassword})
				if (loginResponse.success) {
					// Login successful
					const userData = AuthService.getUserClaims();
					AuthService.applyUserDataToContext(userData, dispatch);
					updateTime()
				} else {
					// Login failed
					message.error("Error while updating cookie")
				}
				formikRef.current.resetForm();
			} else {
				message.error(response?.message ?? "Unexpected error")
			}
		} catch (error){
			message.error(error?.message ?? "Unexpected error")
		}
		setIsSubmitting(false);
	};

	const handleEditUserOk = () => {
		setIsEditUserModalOpen(false);
		formikRef.current.resetForm();
	};

	const handleEditUserCancel = () => {
		setIsEditUserModalOpen(false);
		formikRef.current.resetForm();
	};

	const handleEditAvatarOk = async () => {
		if (fileList.length > 0) {
			setIsSubmitting(true)
			await ProfileService.putImage(finalId, fileList[0]);
			setAvatarUrl(`${userImagesApiUrl}/${finalId}?${Date.now()}`);
			setFileList([]);
			setIsEditAvatarModelOpen(false);
			setIsSubmitting(false)
		} else{
			message.warning("Upload an image!")
		}
	};

	const handleEditAvatarCancel = () => {
		setFileList([]);
		setIsEditAvatarModelOpen(false);
	};

	let profilePageContent;
	if (userId === finalId){
		profilePageContent = (
			<>
				<div style={{display: "flex", justifyContent: "space-evenly", marginTop: "1vh"}}>
					<Button onClick={() => setIsEditAvatarModelOpen(true)}>Change Image</Button>
					<Modal
						title="Upload Image"
						open={isEditAvatarModalOpen}
						onOk={handleEditAvatarOk}
						onCancel={handleEditAvatarCancel}
					>
						<Upload
							beforeUpload={(file) => {
								setFileList([file]);
								return false;
							}}
							fileList={fileList}
						>
							<Button icon={<UploadOutlined />} loading={isSubmitting}>Select File</Button>
						</Upload>
					</Modal>

					<Button
						onClick={() => {
							Modal.confirm({
								title: 'Are you sure you want to delete the image?',
								onOk: async () => {
									await ProfileService.deleteImage(finalId);
									setAvatarUrl(`${userImagesApiUrl}/${finalId}?${Date.now()}`);
								},
							});
						}}
					>
						Delete Image
					</Button>
				</div>

				<h3>
					<KeyValueTable key={`userDataTable_of${finalId}_on_${lastUpdateTime}`} item={userValuesToShow} />
				</h3>

				<Button onClick={() => setIsEditUserModalOpen(true)}>Update Profile</Button>
				<Modal
					title="Update Profile"
					open={isEditUserModalOpen}
					onOk={handleEditUserOk}
					onCancel={handleEditUserCancel}
					footer={null}
				>
					<div style={{display: 'flex', justifyContent: 'center', overflow: 'auto'}}>
						<Formik
							initialValues={{
								name: userData?.values?.[0]?.name ?? '',
								surname: userData?.values?.[0]?.surname ?? '',
								email: userData?.values?.[0]?.email ?? '',
								birthDate: userData?.values?.[0]?.birthDate ?? '',
								currentPassword: '',
								newPassword: '',
								confirmPassword: '',
							}}
							innerRef={formikRef}
							validationSchema={EditProfileSchema}
							onSubmit={handleEditUserSubmit}
						>
							{({ errors, touched }) => (
								<Form style={{width: '100%'}}>
									<MarginedLabel htmlFor="name">Name</MarginedLabel>
									<Field id="name" name="name"
									       as={Input} autoComplete={'given-name'}/>
									{errors.name && touched.name ? <Error>{errors.name}</Error> : null}

									<MarginedLabel htmlFor="surname">Surname</MarginedLabel>
									<Field id="surname" name="surname"
									       as={Input} autoComplete={'last-name'}/>
									{errors.surname && touched.surname ? <Error>{errors.surname}</Error> : null}

									<MarginedLabel htmlFor="email">Email</MarginedLabel>
									<Field id="email" name="email" type="email"
									       as={Input} autoComplete={'email'}/>
									{errors.email && touched.email ? <Error>{errors.email}</Error> : null}

									<MarginedLabel htmlFor="birthDate">Birth Date</MarginedLabel>
									<Field id="birthDate" name="birthDate" type="date"
									       as={Input} autoComplete={'birth-date'}/>
									{errors.birthDate && touched.birthDate ? (
										<Error>{errors.birthDate}</Error>
									) : null}

									<MarginedLabel htmlFor="currentPassword">Current Password
										<span style={{ color: 'red' }}> * </span>
									</MarginedLabel>
									<Field id="currentPassword" name="currentPassword" type="password"
									       as={Input.Password} autoComplete={'current-password'}/>
									{errors.currentPassword && touched.currentPassword ? (
										<Error>{errors.currentPassword}</Error>
									) : null}

									<MarginedLabel htmlFor="newPassword">	New Password </MarginedLabel>
									<Field id="newPassword" name="newPassword" type="password"
									       as={Input.Password} autoComplete={'password'}/>
									{errors.newPassword && touched.newPassword ? (
										<Error>{errors.newPassword}</Error>
									) : null}

									<MarginedLabel htmlFor="confirmPassword">Confirm New Password</MarginedLabel>
									<Field id="confirmPassword" name="confirmPassword" type="password"
									       as={Input.Password} autoComplete={'password'}/>
									{errors.confirmPassword && touched.confirmPassword ? (
										<Error>{errors.confirmPassword}</Error>
									) : null}

									<MarginedButton type="primary" htmlType="submit" loading={isSubmitting}>Update</MarginedButton>
								</Form>
							)}
						</Formik>
					</div>
				</Modal>
			</>
		)
	} else {
		profilePageContent = (
			<h3>
				<KeyValueTable key={`userDataTable_of${finalId}_on_${lastUpdateTime}`} item={userValuesToShow} />
			</h3>
		)
	}

	return (
		<div className={styles.centeredInfoBlock}>
			<h1>Profile</h1>
			<Image
				key={avatarUrl}
				imageClassName={`${styles.profileAvatarSize} ${styles.smoothBorder}`}
				containerClassName={`${styles.backgroundHighlighted} ${styles.smoothBorder}`}
				src={avatarUrl}
				defaultImage={defaultUserIcon}
			/>
			{profilePageContent}
		</div>
	);
}