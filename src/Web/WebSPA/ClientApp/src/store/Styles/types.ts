export interface TOGGLE_DARK_MODE_ACTION {
  type: stylesActionTypes.TOGGLE_DARK_MODE;
  payload?: boolean;
}
export enum stylesActionTypes {
  TOGGLE_DARK_MODE
}

export type stylesActions = TOGGLE_DARK_MODE_ACTION;
