import { Reducer } from "redux";
import { PhrasesState, PhrasesActionTypes } from "./types";

export const initialState: PhrasesState = {
  data: []
};

const reducer: Reducer<PhrasesState> = (state = initialState, action) => {
  switch (action.type) {
    case PhrasesActionTypes.GETALL: {
      return { ...state, data: action.payload };
    }
    default: {
      return state;
    }
  }
};

export { reducer as phrasesReducer };
