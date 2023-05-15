import { useState, useEffect } from 'react';
import {Modal, Table, Button, message} from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { BasketService } from '../../../services/basketService';
import {useDispatch, useSelector} from "react-redux";
import {updateBasketData} from "../../../context/store";

export function Basket() {
	const [open, setOpen] = useState(false);
	const {values, success} = useSelector(state => state.basketData)
	const dispatch = useDispatch();
	const [total, setTotal] = useState(0);

	useEffect(() => {
		async function fetchData() {
			const totalResponse = await BasketService.getTotal();
			setTotal(totalResponse.values[0])
		}
		fetchData();
	}, [dispatch, values, success]);

	const handleRemove = async (gameId) => {
		await BasketService.removeItem(gameId);
		const basketResponse = await BasketService.getBasket();
		dispatch(updateBasketData(basketResponse));
	};

	const handleConfirmPurchase = async () => {
		await BasketService.clear();
		dispatch(updateBasketData({}))
		setOpen(false);

		message.success('Purchase confirmed!');
		setTimeout(() => {
			message.success(`Total price of purchase: ${total}`);
		}, 3000);
	};

	const columns = [
		{
			title: 'Game Name',
			dataIndex: 'gameName',
			key: 'gameName',
			align: 'center'
		},
		{
			title: 'Game Price',
			dataIndex: 'gamePrice',
			key: 'gamePrice',
			align: 'center'
		},
		{
			title: 'Action',
			key: 'action',
			render: (text, record) => (
				<Button danger={true} onClick={() => handleRemove(record.gameId)}>Remove</Button>
			),
			align: 'center'
		},
	];

	return (
		<>
			<ShoppingCartOutlined style={{fontSize: '30px', margin: 'auto'}} onClick={() => setOpen(true)} />
			<Modal
				style={{minWidth: '80vw'}}
				title="Basket"
				open={open}
				onCancel={() => setOpen(false)}
				footer={[
					<Button key="back" onClick={() => setOpen(false)}>
						Cancel
					</Button>,
					<Button key="submit" type="primary" onClick={handleConfirmPurchase}
					        disabled={values?.length === 0 ?? 0}>
						Confirm Purchase
					</Button>,
				]}
			>
				<Table
					key={`${values.length}_${success}`}
					columns={columns}
					dataSource={values?.map((item) => ({ ...item, key: item.gameId }))}
					pagination={false}
				/>
				<h1 style={{margin: 'auto 20px auto auto', width: 'fit-content'}}>Total Price: {total}</h1>
			</Modal>
		</>
	);
}