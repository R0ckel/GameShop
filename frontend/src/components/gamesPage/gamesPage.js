import {CompaniesService} from "../../services/domain/companiesService";
import {GameGenresService} from "../../services/domain/gameGenresService";
import {GamesService} from "../../services/domain/gamesService";

import { useState, useEffect } from 'react';
import {CheckListHeader} from "./checkTable/checkListHeader";
import {BasketService} from "../../services/domain/basketService";
import CheckTableHeader from "./checkTable/checkTableHeader";
import styles from "../../css/app.module.css"
import CheckTableRow from "./checkTable/checkTableRow";
import {useDispatch, useSelector} from "react-redux";
import {gameImagesApiUrl} from "../../variables/connectionVariables";
import defaultGameThumbnail from '../../image/game-icon.png';
import {Button, Form, Input, Pagination, Select} from 'antd';
import {basketUpdated} from "../../context/store";

export function GamesPage() {
	const [companies, setCompanies] = useState({});
	const [genres, setGenres] = useState({});
	const [games, setGames] = useState({});
	const basketData = useSelector(state => state.basketData);
	const {isLoggedIn} = useSelector(state => state.userData);
	const [gameCards, setGameCards] = useState([]);
	const dispatch = useDispatch();
	const cardViewFields = ['name', 'companyName', 'genres', 'price'];

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

	useEffect(() => {
		async function fetchData() {
			const companiesResponse = await CompaniesService.getCards();
			const genresResponse = await GameGenresService.getCards();

			setCompanies(companiesResponse);
			setGenres(genresResponse);
		}

		fetchData();
	}, [dispatch, isLoggedIn]);

	async function toggleBasketItem(gameId, checked){
		if (checked) {
			await BasketService.addItem(gameId)
		}
		else {
			await BasketService.removeItem(gameId)
		}
		dispatch(basketUpdated());
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
			<CheckListHeader shown={gameCards?.length}
			                 key={`clHeader_${basketData?.basketItems?.length}_${basketData?.basketSuccess}`}
			                 selected={basketData?.basketItems?.length ?? 0}/>

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
				<CheckTableHeader withImage={true} cardViewFields={cardViewFields} template={gameCards[0]}/>
				<tbody>
					{gameCards.map(item => (
						<CheckTableRow
							thumbnailSrc={`${gameImagesApiUrl}/${item.id}?thumbnail=true&${Date.now()}`}
							defaultThumbnailSrc={defaultGameThumbnail}
							isChecked={basketData?.basketItems?.filter(x => x.gameId === item.id ).length > 0}
							item={item}
							cardViewFields={cardViewFields}
							key={`${item.id.toString()}_${basketData?.basketItems?.filter(x => x.gameId === item.id)[0]}`}
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
				onChange={value => handleFilterChange('page', value)}
				onShowSizeChange={(current, size) => handleFilterChange('pageSize', size)}
				showSizeChanger
				pageSizeOptions={['2', '5', '10', '20', '50']}
				style={{backgroundColor: 'rgba(255, 255, 255, 0.3)'}}
				className={styles.smoothBorder}
			/>
		</div>
	);
}