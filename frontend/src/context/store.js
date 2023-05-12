import {configureStore, createSlice} from '@reduxjs/toolkit'

const baseAppUrlSlice = createSlice({
	name: 'baseAppUrl',
	initialState: {
		value: ""
	},
	reducers: {
		setBaseUrl(state, action) {
			state.baseUrl = action.payload
		}
	}
})

const productListSlice = createSlice({
	name: 'productList',
	initialState: {
		categoryName: "No category",
		allCategories: [],
		products: [],
		cardViewFields: [],
	},
	reducers: {
		setProducts(state, action) {
			state.products = action.payload
		},
		addProduct(state, action) {
			state.products = [...state.products, action.payload]
		},
		updateProduct(state, action) {
			state.products = state.products.map(product => {
				return product.id === action.payload.id ? action.payload : product
			})
		},
		deleteProduct(state, action) {
			state.products = state.products.filter(product => product.id !== action.payload)
		},
		toggleProductSelected(state, action) {
			const product = state.products.find(product => product.id === action.payload)
			if (product) {
				product.selected = !product.selected
			}
		},
		unselectAll(state) {
			state.products.map(product => {
				product.selected = false;
				return product;
			})
		},
		setCardViewFields(state, action) {
			state.cardViewFields = action.payload
		},
	},
})

const userStatusSlice = createSlice({
	name: 'userStatus',
	initialState: {
		isLoggedIn: false,
		username: "",
	},
	reducers: {
		setLoggedInValue(state, action) {
			state.isLoggedIn = action.payload.value
			state.username = action.payload.username
		},
	}
})

export const store = configureStore({
	reducer: {
		baseAppUrl: baseAppUrlSlice.reducer,
		productList: productListSlice.reducer,
		userStatus: userStatusSlice.reducer,
	}
})

// експортуємо дії з кожного slice
export const {setBaseUrl} = baseAppUrlSlice.actions
export const {setLoggedInValue} = userStatusSlice.actions
export const {
	setProducts,
	addProduct,
	updateProduct,
	deleteProduct,
	toggleProductSelected,
	setCardViewFields
} = productListSlice.actions

// експортуємо додаткові розрахункові поля
export const selectedProductsCount = state => state.productList.products.filter(product => product.selected).length
export const getProductCategories = state => {
	const categories = state.productList.products.map(product => product.category);
	return [...new Set(categories)];
};