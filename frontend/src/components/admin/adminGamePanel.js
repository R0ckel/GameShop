import React, {useEffect, useState} from 'react';
import {Button, Form, Input, Input as AntdInput, List, message, Modal, Pagination, Switch, Table, Upload} from 'antd';
import {CloseOutlined, DeleteOutlined, EditOutlined, PlusOutlined, UploadOutlined} from '@ant-design/icons';
import {Form as FormikForm, Formik, useField} from 'formik';
import * as Yup from 'yup';
import Image from '../helpers/image'
import styled from "styled-components";
import defaultGameIcon from '../../image/game-icon.png';
import styles from '../../css/app.module.css';
import {GamesService} from "../../services/domain/gamesService";
import {CompaniesService} from "../../services/domain/companiesService";
import {GameGenresService} from "../../services/domain/gameGenresService";
import {gameImagesApiUrl} from "../../variables/connectionVariables";
import { Form as AntdForm, Select } from 'antd';
import {useSelector} from "react-redux";
import {ErrorPage} from "../responses/errorPage";

const { Option } = Select;

const FormikInput = ({label, ...props}) => {
	const [field, meta] = useField(props);
	return (
		<>
			<MarginedLabel>
				{label}
				<AntdInput {...field} {...props} />
			</MarginedLabel>
			{meta.touched && meta.error ? <Error>{meta.error}</Error> : null}
		</>
	);
};

const FormikAntdSelect = ({ label, options, ...props }) => {
	const [field, meta, helpers] = useField(props);
	const { setValue } = helpers;

	return (
		<div style={{
			margin: '10px 0 2px',
		}}>
			<AntdForm.Item label={label}>
				<Select
					{...field}
					{...props}
					onChange={value => setValue(value)}
					style={{ minWidth: '180px' }}
				>
					{options.map(option => (
						<Option key={option.id} value={option.id}>
							{option.name}
						</Option>
					))}
				</Select>
			</AntdForm.Item>
			{meta.touched && meta.error ? <Error>{meta.error}</Error> : null}
		</div>
	);
};

const FormikAntdMultiSelect = ({ label, options, ...props }) => {
	const [field, meta, helpers] = useField(props);
	const { setValue } = helpers;

	return (
		<AntdForm.Item label={label} style={{margin: '10px 0 2px'}}>
			<Select
				mode="multiple"
				{...field}
				{...props}
				onChange={value => setValue(value)}
				style={{ minWidth: '180px' }}
			>
				{options.map(option => (
					<Option key={option.id} value={option.id}>
						{option.name}
					</Option>
				))}
			</Select>
			{meta.touched && meta.error ? <Error>{meta.error}</Error> : null}
		</AntdForm.Item>
	);
};

const MarginedLabel = styled("label")`
  margin: 10px 0 2px;
`;

const MarginedButton = styled(Button)`
  margin-top: 20px;
`;

const Error = styled("div")`
  color: red;
`;

export const AdminGamePanel = () => {
	const [visible, setVisible] = useState(false);
	const [editingProduct, setEditingProduct] = useState(null);
	const [tableView, setTableView] = useState(localStorage.getItem('tableView') === 'true');

	const [games, setGames] = useState({});
	const [companies, setCompanies] = useState({});
	const [genres, setGenres] = useState({});
	const [lastUpdate, setLastUpdate] = useState(Date.now())

	const [filters, setFilters] = useState({
		companyName: '',
	});
	const handleFilterChange = async (key, value) => {
		setFilters(prevFilters => ({ ...prevFilters, [key]: value }));
	};
	const [extraFiltersShown, setExtraFiltersShown] = useState(false);
	const [toggleButtonText, setToggleButtonText] = useState('Show extra filters')
	const handleReset = () => {
		setFilters({});
	};
	const toggleExtraFilters = () =>{
		const isShown = extraFiltersShown;
		setExtraFiltersShown(!isShown);
		setToggleButtonText(isShown ? 'Show extra filters' : 'Hide extra filters')
	}

	const [fileList, setFileList] = useState([]);
	const [isEditImageModalOpen, setIsEditImageModalOpen] = useState(false);
	const [isSubmitting, setIsSubmitting] = useState(false);

	const [currentGameId, setCurrentGameId] = useState(null);
	const [currentImageId, setCurrentImageId] = useState('');

	useEffect( () => {
		async function updateGames(){
			setGames(await GamesService.get(filters));
		}
		updateGames()
	}, [filters, lastUpdate])

	useEffect(() => {
		async function fetchData() {
			const companiesResponse = await CompaniesService.getCards();
			const genresResponse = await GameGenresService.getCards();

			setCompanies(companiesResponse);
			setGenres(genresResponse);
		}

		fetchData();
	}, []);

	useEffect(() => {
		localStorage.setItem('tableView', tableView.toString());
	}, [tableView]);

	const {isLoggedIn, role} = useSelector(state => state.userData)
	if (!isLoggedIn || role.toLowerCase() !== "admin"){
		return <ErrorPage code={401}/>
	}

	const columns = [
		{
			title: 'Image',
			dataIndex: 'id',
			align: 'center',
			render: (record) => {
				return <Image
					src={`${gameImagesApiUrl}/${record}?timeStamp=${lastUpdate}`}
					defaultImage={defaultGameIcon}
					alt={record?.name}
					imageClassName={`${styles.thumbnail} ${styles.smoothBorder}`}
					onClick={() => handleOpenUploadImageForm(record)}
				/>
			},
		},
		{
			title: 'Name',
			dataIndex: 'name',
			align: 'center',
		},
		{
			title: 'Company',
			dataIndex: 'companyName',
			align: 'center',
		},
		{
			title: 'Description',
			dataIndex: 'description',
			align: 'center',
		},
		{
			title: 'Genres',
			dataIndex: 'genres',
			align: 'center',
			render: (record) => {
				return genreListToString(record)
			}
		},
		{
			title: 'Price',
			dataIndex: 'price',
			align: 'center',
		},
		{
			title: 'Actions',
			align: 'center',
			render: (_, record) => (
				<>
					<EditOutlined onClick={() => handleOpenEditForm(record)} type="primary">
						Edit
					</EditOutlined>
					<DeleteOutlined onClick={() => handleDelete(record)} danger="true" style={{marginLeft: '1vw'}}>
						Delete
					</DeleteOutlined>
					<CloseOutlined onClick={() => handleDeleteImage(record)} danger='true' style={{marginLeft: '1vw'}}>
						Delete Image
					</CloseOutlined>
				</>
			),
		},
	];

	const genreListToString = (genres) => {
		return genres.map(x => x.name).join(', ');
	}

	const handleOpenAddForm = () => {
		setEditingProduct(null);
		setVisible(true);
	};

	const handleOpenEditForm = (record) => {
		setEditingProduct(record);
		setVisible(true);
		setCurrentGameId(record.id)
	};

	const handleCancel = () => {
		setVisible(false);
		setCurrentGameId(null)
	};

	const onFinish = async (values) => {
		let response;
		if (currentGameId != null) {
			response = await GamesService.edit({...values, genreIds: values.genreIds.map(x=> x.value), id: currentGameId})
		} else {
			response = await GamesService.add({...values})
		}
		if (response?.success){
			setLastUpdate(Date.now())
			setVisible(false)
		}
		else {
			message.error("Error occurred! Message:" + response?.message)
		}
	}

	const handleDelete = (record) => {
		Modal.confirm({
			title: `Are you sure you want to delete ${record?.name ?? 'this game'}?`,
			onOk: async () => {
				await GamesService.delete(record.id);
				setLastUpdate(Date.now())
			},
		});
	};

	//image
	const handleOpenUploadImageForm = (record) => {
		setCurrentImageId(record);
		setIsEditImageModalOpen(true);
	}

	const handleEditImageOk = async () => {
		if (fileList.length > 0) {
			setIsSubmitting(true)
			const response = await GamesService.putImage(currentImageId, fileList[0]);
			if (response.data?.success){
				setFileList([]);
				setIsEditImageModalOpen(false);
				setLastUpdate(Date.now())
			}
			else {
				message.error("Error occurred! Try again...")
			}
			setIsSubmitting(false);
		} else {
			message.warning("Upload an image!")
		}
	};

	const handleDeleteImage = (record) => {
		Modal.confirm({
			title: `Are you sure you want to delete the image of ${record?.name ?? 'this game'}?`,
			onOk: async () => {
				await GamesService.deleteImage(record.id);
				setLastUpdate(Date.now())
			},
		});
	}

	const handleEditImageCancel = () => {
		setFileList([]);
		setIsEditImageModalOpen(false);
	};
	//image

	const maxPrice = 1_000_000
	const validationSchema = Yup.object({
		name: Yup.string().required().max(255, 'Name must be 255 characters long or less!'),
		companyId: Yup.string().required('Choose company for the game!'),
		description: Yup.string().required().max(5000, 'Description must be 5000 characters long or less!'),
		price: Yup.number().required()
		.positive(`Price must be positive number`)
		.max(maxPrice, `Price can\`t be above ${maxPrice}`)
	});

	const imageForm = (
		<Modal
			title="Upload Image"
			open={isEditImageModalOpen}
			onOk={handleEditImageOk}
			onCancel={handleEditImageCancel}
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
	)

	const productForm = (
		<Modal
			title={"Game Form"}
			open={visible}
			onCancel={handleCancel}
			forceRender
			footer={null}
			width={'90vw'}
		>
			<div style={{display: 'flex', justifyContent: 'center', width: '100%'}}>
				<Formik
					initialValues={{
						name: editingProduct?.name ?? '',
						companyId: editingProduct?.companyId ?? '',
						description: editingProduct?.description ?? '',
						genreIds: editingProduct?.genres?.map(x => ({key: x.name, value: x.id})) ?? [],
						price: editingProduct?.price ?? 0
					}}
					validationSchema={validationSchema}
					onSubmit={onFinish}
					enableReinitialize
				>
					{({handleSubmit} ) => (
						<FormikForm style={{ width: '100%' }}>
							<FormikInput label="Name" name="name" />
							<FormikInput label="Description" name="description" />
							<FormikAntdSelect label="Company" name="companyId" options={companies?.values ?? []} />
							<FormikAntdMultiSelect label="Genres" name="genreIds" options={genres?.values ?? []} />
							<FormikInput label="Price" name="price" type="number" />
							<div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
								<MarginedButton onClick={handleCancel} style={{ marginRight: '1rem' }}>
									Cancel
								</MarginedButton>
								<MarginedButton onClick={handleSubmit} type="primary">
									Submit
								</MarginedButton>
							</div>
						</FormikForm>
					)}
				</Formik>
			</div>
		</Modal>
	);

	const FilterForm = (
		<Form layout="inline" className={styles.filterForm}>
			<div className={styles.inlineForm}>
				<Form.Item label="Name" style={{flexGrow: '1', minWidth: '50vw'}}>
					<Input
						value={filters.name}
						onChange={e => handleFilterChange('name', e.target.value)}
					/>
				</Form.Item>
				<Form.Item>
					<Button onClick={toggleExtraFilters} style={{minWidth: '10vw'}}>{toggleButtonText}</Button>
				</Form.Item>
				<Form.Item>
					<Button onClick={handleReset} style={{minWidth: '10vw'}}>Reset</Button>
				</Form.Item>
			</div>
			{!extraFiltersShown ? <></> :
				<div className={styles.inlineForm}>
					<Form.Item label="Price From">
						<Input
							type="number"
							value={filters.priceFrom}
							onChange={e => handleFilterChange('priceFrom', e.target.value)}
						/>
					</Form.Item>
					<Form.Item label="Price To">
						<Input
							type="number"
							value={filters.priceTo}
							onChange={e => handleFilterChange('priceTo', e.target.value)}
						/>
					</Form.Item>
					<Form.Item label="Company">
						<Select
							value={filters.companyName}
							onChange={value => handleFilterChange('companyName', value)}
							style={{minWidth: '180px'}}
						>
							<Option value="">Any company</Option>
							{companies?.values?.map(company => (
								<Option key={company.id} value={company.name}>
									{company.name}
								</Option>
							))}
						</Select>
					</Form.Item>
					<Form.Item label="Genres">
						<Select
							mode="multiple"
							value={filters.genres}
							style={{minWidth: '180px'}}
							onChange={value => handleFilterChange('genreIds', value)}
						>
							{genres?.values?.map(genre => (
								<Option key={genre.id} value={genre.id}>
									{genre.name}
								</Option>
							))}
						</Select>
					</Form.Item>
				</div>
			}
		</Form>
	)

	return (
		<div
			style={{
				display: 'flex',
				flexDirection: 'column',
				justifyContent: 'center',
				textAlign: 'center',
				width: '100%',
				padding: '1vh 6vw',
			}}
		>
			<h1 style={{marginTop: '0px', marginBottom: '10px'}}>ADMIN PANEL - GAMES</h1>
			<div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
				<Button
					onClick={handleOpenAddForm}
					type="primary"
					icon={<PlusOutlined/>}
					style={{
						backgroundColor: '#287c00',
						borderColor: '#52c41a',
						margin: '1vh ',
					}}
				>
					Add Product
				</Button>
				<Switch checked={tableView} onChange={(checked) => setTableView(checked)}/>
			</div>
			{FilterForm}
			<Pagination
				current={games?.pageNumber}
				total={!isNaN(games?.pageCount * games?.pageSize)
					? games?.pageCount * games?.pageSize
					: 1 }
				pageSize={games?.pageSize ?? 10}
				onChange={value => handleFilterChange('page', value)}
				onShowSizeChange={(current, size) => handleFilterChange('pageSize', size)}
				showSizeChanger
				pageSizeOptions={['2', '5', '10', '20', '50']}
				style={{backgroundColor: 'rgba(255, 255, 255, 0.3)'}}
				className={`${styles.smoothBorder} ${styles.centered}`}
			/>

			{tableView ? (
				<>
					<Table dataSource={games?.values ?? []} columns={columns}
					       rowKey="id"
					       style={{backgroundColor: 'rgba(255,255,255,0.5)'}}
					       pagination={false}
					/>
				</>
			) : (
				<>
					<List
						itemLayout="horizontal"
						dataSource={games?.values ?? []}
						style={{
							backgroundColor: 'rgba(255,255,255)',
							padding: '20px',
							borderRadius: '10px'
						}}
						renderItem={(item) => (
							<List.Item
								actions={[
									<EditOutlined onClick={() => handleOpenEditForm(item)} type="primary" style={{marginRight: '1vw'}}/>,
									<DeleteOutlined onClick={() => handleDelete(item)} danger="true"/>,
								]}
							>
								<List.Item.Meta
									avatar={
										<Image
											src={`${gameImagesApiUrl}/${item.id}`}
											defaultImage={defaultGameIcon}
											alt={item.name}
											imageClassName={`${styles.thumbnail} ${styles.smoothBorder}`}
											onClick={() => handleOpenEditForm(item)}
										/>
									}
									title={`${item.name} - ${item.companyName}; ${item.price}$`}
									description={`${item.description}`}
								/>
							</List.Item>
						)}
					/>
				</>
			)}
			{productForm}
			{imageForm}
		</div>
	);
}