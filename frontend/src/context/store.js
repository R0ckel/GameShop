import {configureStore, createSlice} from '@reduxjs/toolkit'

const userDataSlice = createSlice({
	name: 'userStatus',
	initialState: {
		isLoggedIn: false,
		userId: "",
		username: "",
		email: "",
		role: ""
	},
	reducers: {
		setUserData(state, action) {
			state.isLoggedIn = action.payload.isLoggedIn ?? false
			state.userId = action.payload.userId ?? ""
			state.username = action.payload.username ?? ""
			state.email = action.payload.email ?? ""
			state.role = action.payload.role ?? "unauthorized"
		},
	}
})

const basketDataSlice = createSlice({
	name: 'basketData',
	initialState: {
		basketItems: [],
		basketSuccess: false,
		basketItemsCount: 0,
		lastLocalBasketUpdateTime: null
	},
	reducers: {
		updateBasketData(state, action){
			state.basketItems = action.payload?.values ?? [];
			state.basketSuccess = action.payload?.success ?? false;
			state.basketItemsCount = action.payload?.valueCount ?? 0;
		},
		basketUpdated(state){
			state.lastLocalBasketUpdateTime = Date.now()
		}
	}
})

export const store = configureStore({
	reducer: {
		userData: userDataSlice.reducer,
		basketData: basketDataSlice.reducer
	}
})

export const {setUserData} = userDataSlice.actions
export const {updateBasketData, basketUpdated} = basketDataSlice.actions