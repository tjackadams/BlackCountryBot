import { ActionCreator } from "redux";
import {
  PhrasesListUpdatedAction,
  IPhrase,
  TopPhrasesListUpdatedAction
} from "./types";

export const updatePhrasesList: ActionCreator<PhrasesListUpdatedAction> = (
  phrases: IPhrase[]
) => ({
  type: "@@phrase/PHRASES_LIST_UPDATED",
  payload: {
    phrases: phrases
  }
});

export const updatedTopPhrasesList: ActionCreator<
  TopPhrasesListUpdatedAction
> = (phrases: IPhrase[]) => ({
  type: "@@phrase/TOP_PHRASES_LIST_UPDATED",
  payload: {
    phrases: phrases
  }
});
