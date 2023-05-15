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

export const store = configureStore({
	reducer: {
		userData: userDataSlice.reducer,
	}
})

export const {setUserData} = userDataSlice.actions