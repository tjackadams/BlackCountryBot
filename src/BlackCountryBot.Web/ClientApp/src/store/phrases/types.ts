export interface Phrase {
  phraseId: number;
  original: string;
  translation: string;
  lastTweetTime: Date;
  numberOfTweets: number;
}

export enum PhrasesActionTypes {
  GETALL = "@@phrases/GETALL"
}

export interface PhrasesState {
  readonly data: Phrase[];
}
