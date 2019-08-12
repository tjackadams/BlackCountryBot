import { IColumn } from "office-ui-fabric-react";
import { Phrase } from "../../store/phrases/types";

export interface IPhraseListState {
  columns: IColumn[];
  items: Phrase[];
  selectionDetails: string;
  showModal: boolean;
}

export interface IPhraseListProps {}

export interface PropsFromState {
  data: Phrase[];
}

export interface PropsFromDispatch {}
