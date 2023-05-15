import {CompaniesService} from "../../services/companiesService";
import {GameGenresService} from "../../services/gameGenresService";
import {GamesService} from "../../services/gamesService";

import { useState, useEffect } from 'react';
import {CheckListHeader} from "./checkTable/checkListHeader";
import {BasketService} from "../../services/basketService";
import CheckTableHeader from "./checkTable/checkTableHeader";
import styles from "../../css/app.module.css"
import CheckTableRow from "./checkTable/checkTableRow";
import {useSelector} from "react-redux";
import {gameImagesApiUrl} from "../../variables/connectionVariables";
import defaultGameThumbnail from '../../image/game-icon.png';
import {Button, Form, Input, Pagination, Select} from 'antd';

export function GamesPage() {
	const [companies, setCompanies] = useState({});
	const [genres, setGenres] = useState({});
	const [games, setGames] = useState({});
	const [basketData, setBasketData] = useState({});
	const [itemsSelected, setItemsSelected] = useState(0);
	const {isLoggedIn} = useSelector(state => state.userData);
	const [gameCards, setGameCards] = useState([]);

	//filters
	const { Option } = Select;
	const [filters, setFilters] = useState({
		companyName: '',
	});
	const [extraFiltersShown, setExtraFiltersShown] = useState(false);
	const [toggleButtonText, setToggleButtonText] = useState('Show extra filters')

	const toggleExtraFilters = () =>{
		const isShown = extraFiltersShown;
		setExtraFiltersShown(!isShown);
		setToggleButtonText(isShown ? 'Show extra filters' : 'Hide extra filters')
	}

	const handleFilterChange = async (key, value) => {
		if (key === 'page'){
			setFilters(prevFilters => ({ ...prevFilters, [key]: value }));
		}
		else {
			setFilters(prevFilters => ({ ...prevFilters, [key]: value, page: 1 }));
		}
	};
	
	useEffect( () => {
		async function updateGames(){
			setGames(await GamesService.get(filters));
		}
		updateGames()
	}, [filters])

	const handleReset = () => {
		setFilters({});
	};
	//filters

	//pagination
	const handlePageChange = async (page) => {
		await handleFilterChange('page', page);
	};

	const handlePageSizeChange = async (current, size) => {
		await handleFilterChange('pageSize', size);
	};
	//pagination

	useEffect(() => {
		async function fetchData() {
			const companiesResponse = await CompaniesService.getCards();
			const genresResponse = await GameGenresService.getCards();
			const gamesResponse = await GamesService.get();

			setCompanies(companiesResponse);
			setGenres(genresResponse);
			setGames(gamesResponse);

			if (isLoggedIn) {
				const basketDataResponse = await BasketService.getBasket();
				setBasketData(basketDataResponse);
				if (basketDataResponse?.valueCount != null) setItemsSelected(basketDataResponse.valueCount)
			}
		}

		fetchData();
	}, [isLoggedIn]);

	async function toggleBasketItem(gameId, checked){
		if (checked) {
			await BasketService.addItem(gameId)
			setItemsSelected(itemsSelected + 1)
		}
		else {
			await BasketService.removeItem(gameId)
			setItemsSelected(itemsSelected - 1)
		}
	}

	useEffect(() => {
		setGameCards(
			!games?.hasOwnProperty("values")
				? []
				: games.values.map((game) => {
					const genreNames = game.genres.map((genre) => genre.name);
					const genresString =
						genreNames.length > 3
							? `${genreNames.slice(0, 3).join(", ")} ...`
							: genreNames.join(", ");

					return {
						id: game.id,
						name: game.name,
						price: game.price,
						companyName: game.companyName,
						genres: genresString,
					};
				}));
	}, [games])
	return (
		<div className={styles.centeredInfoBlock} style={{width: '90%'}}>
			<CheckListHeader shown={gameCards?.length} selected={itemsSelected}/>

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

			<table className={styles.smoothTable}>
				<CheckTableHeader withImage={true} cardViewFields={['name', 'company', 'genres', 'price']} template={gameCards[0]}/>
				<tbody>
					{gameCards.map(item => (
						<CheckTableRow
							thumbnailSrc={`${gameImagesApiUrl}/${item.id}?thumbnail=true&${Date.now()}`}
							defaultThumbnailSrc={defaultGameThumbnail}
							isChecked={basketData?.values?.filter(x => x.gameId === item.id ).length > 0}
							item={item}
							cardViewFields={['name', 'company', 'genres', 'price']}
							key={`${item.id.toString()}_${basketData?.success}`}
							updateSender={toggleBasketItem}
						/>
					))}
				</tbody>
			</table>

			<Pagination
				current={games?.pageNumber}
				total={!isNaN(games?.pageCount * games?.pageSize)
					? games?.pageCount * games?.pageSize
					: 1 }
				pageSize={games?.pageSize ?? 10}
				onChange={handlePageChange}
				onShowSizeChange={handlePageSizeChange}
				showSizeChanger
				pageSizeOptions={['2', '5', '10', '20', '50']}
				style={{backgroundColor: 'rgba(255, 255, 255, 0.3)'}}
				className={styles.smoothBorder}
			/>
		</div>
	);
}