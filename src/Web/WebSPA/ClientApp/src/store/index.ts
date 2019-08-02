import { combineReducers } from "redux";
import { History } from "history";
import { connectRouter, RouterState } from "connected-react-router";

import { stylesReducer, IStylesStore } from "./Styles";
import { reducer as phraseReducer, PhraseState } from "./Phrases";

export interface ApplicationState {
  phrases: PhraseState;
  styles: IStylesStore;
  router: RouterState;
}

const rootReducer = (history: History) =>
  combineReducers<ApplicationState>({
    phrases: phraseReducer,
    styles: stylesReducer,
    router: connectRouter(history)
  });

export default rootReducer;
