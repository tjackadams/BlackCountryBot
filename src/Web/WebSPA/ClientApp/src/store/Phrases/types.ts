import { Action } from "redux";

export interface PhrasesListUpdatedAction extends Action {
  type: "@@phrase/PHRASES_LIST_UPDATED";
  payload: {
    phrases: IPhrase[];
  };
}

export interface TopPhrasesListUpdatedAction extends Action {
  type: "@@phrase/TOP_PHRASES_LIST_UPDATED";
  payload: {
    phrases: IPhrase[];
  };
}

export enum phrasesActionTypes {
  GETALL_PHRASES,
  GET_TOP_TWEETS,
  SIGNALR_CREATE_PHRASE,
  SIGNALR_DELETE_PHRASE,
  SIGNALR_UPDATE_PHRASE,
  SIGNALR_TWEET_PHRASE
}

export interface IPhrase {
  phraseId: number;
  original: string;
  translation: string;
  numberOfTweets: number;
  lastTweetTime: string;
}
export interface PhraseState {
  all: IPhrase[];
  top5: IPhrase[];
}

export type PhraseActions =
  | PhrasesListUpdatedAction
  | TopPhrasesListUpdatedAction;
