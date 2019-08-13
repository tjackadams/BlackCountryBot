import { combineReducers } from "redux";
import { connectRouter } from "connected-react-router";

import { reducer as phrasesReducer } from "./Phrases";

export const createRootReducer = history =>
  combineReducers({
    phrases: phrasesReducer,
    router: connectRouter(history)
  });
