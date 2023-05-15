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
		values: [],
		success: false,
		valueCount: 0
	},
	reducers: {
		updateBasketData(state, action){
			state.values = action.payload.values ?? [];
			state.success = action.payload.success ?? false;
			state.valueCount = action.payload.valueCount ?? 0;
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
export const {updateBasketData} = basketDataSlice.actions