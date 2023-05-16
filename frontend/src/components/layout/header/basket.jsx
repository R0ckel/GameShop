import { useState, useEffect } from 'react';
import {Modal, Table, Button, message} from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { BasketService } from '../../../services/domain/basketService';
import {useDispatch, useSelector} from "react-redux";
import {updateBasketData} from "../../../context/store";

export function Basket() {
	const [open, setOpen] = useState(false);
	const {basketItems, basketSuccess, lastLocalBasketUpdateTime} = useSelector(state => state.basketData)
	const {isLoggedIn} = useSelector(state => state.userData)
	const dispatch = useDispatch();
	const [total, setTotal] = useState(0);

	useEffect(() => {
		async function updateBasket(){
			if (isLoggedIn) {
				const basketDataResponse = await BasketService.getBasket();
				dispatch(updateBasketData(basketDataResponse));
			}
		}
		updateBasket();
	}, [dispatch, isLoggedIn, lastLocalBasketUpdateTime])

	useEffect(() => {
		async function fetchTotal(){
			const totalResponse = await BasketService.getTotal();
			setTotal(totalResponse?.values[0] ?? 0)
		}
		fetchTotal();
	}, [dispatch, basketItems, basketSuccess]);

	const handleRemove = async (gameId) => {
		await BasketService.removeItem(gameId);
		const basketResponse = await BasketService.getBasket();
		dispatch(updateBasketData(basketResponse));
	};

	const handleConfirmPurchase = async () => {
		Modal.confirm({
			title: `Are you confirming your purchase? (${total}$)`,
			onOk: async () => {
				await BasketService.clear();
				dispatch(updateBasketData({}))
				setOpen(false);

				message.success('Purchase confirmed!');
				setTimeout(() => {
					message.success(`Total price of purchase: ${total}`);
				}, 3000);
			},
		});
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
			render:(record) => (
				`${record}$`
			),
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
			<ShoppingCartOutlined style={{fontSize: '30px', margin: 'auto 1vw'}} onClick={() => setOpen(true)} />
			<Modal
				style={{minWidth: 'calc(min(800px, 80vw))'}}
				title="Basket"
				open={open}
				onCancel={() => setOpen(false)}
				footer={[
					<Button key="back" onClick={() => setOpen(false)}>
						Cancel
					</Button>,
					<Button key="submit" type="primary" onClick={handleConfirmPurchase}
					        disabled={basketItems?.length === 0 ?? 0}>
						Confirm Purchase
					</Button>,
				]}
			>
				<Table
					key={`${basketItems?.length}_${basketSuccess}`}
					columns={columns}
					dataSource={basketItems?.map((item) => ({ ...item, key: item.gameId }))}
					pagination={false}
				/>
				<h1 style={{margin: 'auto 20px auto auto', width: 'fit-content'}}>Total Price: {total}$</h1>
			</Modal>
		</>
	);
}