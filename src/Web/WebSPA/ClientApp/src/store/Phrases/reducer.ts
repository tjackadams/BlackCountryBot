import { PhraseActions, PhraseState } from "./types";
import { Reducer } from "redux";

export const initialState: PhraseState = { all: [], top5: [] };

export const reducer: Reducer<PhraseState> = (
  state: PhraseState = initialState,
  action
) => {
  switch ((action as PhraseActions).type) {
    case "@@phrase/PHRASES_LIST_UPDATED":
      return {
        ...state,
        all: action.payload.phrases
      };
    case "@@phrase/TOP_PHRASES_LIST_UPDATED":
      return {
        ...state,
        top5: action.payload.phrases
      };
    default:
      return state;
  }
};
