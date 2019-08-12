import { action } from "typesafe-actions";
import { PhrasesActionTypes, Phrase } from "./types";

export const getAll = (data: Phrase[]) =>
  action(PhrasesActionTypes.GETALL, data);
