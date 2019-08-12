import { combineReducers, Dispatch, Action, AnyAction } from "redux";
import { connectRouter, RouterState } from "connected-react-router";
import { History } from "history";

import { phrasesReducer } from "./phrases/reducer";
import { PhrasesState } from "./phrases/types";

export interface ApplicationState {
  phrases: PhrasesState;
  router: RouterState;
}

export interface ConnectedReduxProps<A extends Action = AnyAction> {
  dispatch: Dispatch<A>;
}

export const createRootReducer = (history: History) =>
  combineReducers({
    phrases: phrasesReducer,
    router: connectRouter(history)
  });
