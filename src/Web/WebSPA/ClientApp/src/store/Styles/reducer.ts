import { stylesActions, stylesActionTypes } from "./types";

export interface IStylesStore {
  darkMode: boolean;
}

const initialState: IStylesStore = { darkMode: false };

export const stylesReducer = (
  state: IStylesStore = initialState,
  action: stylesActions
) => {
  switch (action.type) {
    case stylesActionTypes.TOGGLE_DARK_MODE:
      return {
        ...state,
        darkMode: !state.darkMode
      };
    default:
      return state;
  }
};
