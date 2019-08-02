import React, { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { format } from "date-fns";
import { ICommandBarItemProps, IColumn } from "office-ui-fabric-react";

import { ApplicationState } from "../../store";
import { IPhrase } from "../../store/Phrases";
import { CreateModal, EditModal, PhraseTable } from "./";

interface PhraseTableContainerState {
  columns: IColumn[];
  items: IPhrase[];
  isDataLoaded: boolean;
  showCreateModal: boolean;
  showEditModal: boolean;
  selectedPhrase?: IPhrase;
}

export const PhraseTableContainer = () => {
  const { all: phrases } = useSelector(
    (state: ApplicationState) => state.phrases
  );

  const [state, setState] = useState<PhraseTableContainerState>({
    columns: [],
    items: [],
    isDataLoaded: false,
    showCreateModal: false,
    showEditModal: false
  });

  const dispatch = useDispatch();

  const openModal = (modal: number) => {
    if (modal === 0) {
      setState(prevState => ({
        ...prevState,
        showCreateModal: true,
        showEditModal: false
      }));
    }

    if (modal === 1) {
      setState(prevState => ({
        ...prevState,
        showCreateModal: false,
        showEditModal: true
      }));
    }
  };

  const closeModal = () => {
    setState(prevState => ({
      ...prevState,
      showCreateModal: false,
      showEditModal: false
    }));
  };

  const commandItems = (): ICommandBarItemProps[] => {
    return [
      {
        key: "addPhrase",
        text: "New phrase",
        iconProps: { iconName: "Add" },
        onClick: () => openModal(0)
      }
    ];
  };

  const onColumnClick = (
    column: IColumn,
    prevState: PhraseTableContainerState
  ): { items: IPhrase[]; columns: IColumn[] } => {
    const { columns, items } = prevState;
    const newColumns = columns.slice();
    const currColumn = newColumns.filter(
      currCol => column.key === currCol.key
    )[0];
    newColumns.forEach(newCol => {
      if (newCol === currColumn) {
        currColumn.isSortedDescending = !currColumn.isSortedDescending;
        currColumn.isSorted = true;
      } else {
        newCol.isSorted = false;
        newCol.isSortedDescending = true;
      }
    });

    const newItems = _copyAndSort(
      items,
      currColumn.fieldName!,
      currColumn.isSortedDescending
    );

    return {
      items: newItems,
      columns: newColumns
    };
  };

  const columns: IColumn[] = [
    {
      key: "column1",
      name: "Phrase",
      fieldName: "original",
      minWidth: 300,
      maxWidth: 400,
      isRowHeader: true,
      isResizable: true,
      isSorted: true,
      isSortedDescending: false,
      sortAscendingAriaLabel: "Sorted A to Z",
      sortDescendingAriaLabel: "Sorted Z to A",
      onColumnClick: (ev: any, column: IColumn) =>
        setState(prevState => {
          const { items, columns } = onColumnClick(column, prevState);
          return {
            ...prevState,
            items,
            columns
          };
        }),
      data: "string",
      isPadded: true,
      isMultiline: true
    },
    {
      key: "column2",
      name: "Translation",
      fieldName: "translation",
      minWidth: 210,
      maxWidth: 400,
      isRowHeader: true,
      isResizable: true,
      isSorted: true,
      isSortedDescending: false,
      sortAscendingAriaLabel: "Sorted A to Z",
      sortDescendingAriaLabel: "Sorted Z to A",
      onColumnClick: (ev: any, column: IColumn) =>
        setState(prevState => {
          const { items, columns } = onColumnClick(column, prevState);
          return {
            ...prevState,
            items,
            columns
          };
        }),
      data: "string",
      isPadded: true,
      isMultiline: true
    },
    {
      key: "column3",
      name: "Times Tweeted",
      fieldName: "numberOfTweets",
      minWidth: 100,
      maxWidth: 120,
      isResizable: true,
      onColumnClick: (ev: any, column: IColumn) =>
        setState(prevState => {
          const { items, columns } = onColumnClick(column, prevState);
          return {
            ...prevState,
            items,
            columns
          };
        }),
      data: "number",
      onRender: (item: { numberOfTweets: React.ReactNode }) => {
        return <span>{item.numberOfTweets}</span>;
      }
    },
    {
      key: "column4",
      name: "Last Tweeted",
      fieldName: "lastTweetTime",
      minWidth: 120,
      maxWidth: 300,
      isResizable: true,
      onColumnClick: (ev: any, column: IColumn) =>
        setState(prevState => {
          const { items, columns } = onColumnClick(column, prevState);
          return {
            ...prevState,
            items,
            columns
          };
        }),
      data: "string",
      isPadded: true,
      onRender: (item: { lastTweetTime: string | number | Date }) => {
        return (
          <span>
            {item.lastTweetTime &&
              format(new Date(item.lastTweetTime), "dd/MM/yy h:mm bbbb")}
          </span>
        );
      }
    }
  ];

  useEffect(() => {
    if (state.items.length !== phrases.length) {
      setState(prevState => ({
        ...prevState,
        items: phrases,
        isDataLoaded: true,
        columns: columns
      }));
    }
  }, [columns, state.items.length, phrases]);

  return (
    <>
      <PhraseTable
        commandItems={commandItems()}
        tableItems={state.items}
        tableColumns={state.columns}
        isReady={state.isDataLoaded}
        dispatch={dispatch}
        openModal={openModal}
      />
      ,
      <CreateModal
        show={state.showCreateModal}
        close={closeModal}
        submit={(item: { original: string; translation: string }) =>
          dispatch({
            type: "SIGNALR_CREATE_PHRASE",
            command: item
          })
        }
      />
      ,
      <EditModal
        show={state.showEditModal}
        close={closeModal}
        phrase={state.selectedPhrase}
        submit={(item: {
          phraseId: number;
          original: string;
          translation: string;
        }) =>
          dispatch({
            type: "SIGNALR_UPDATE_PHRASE",
            command: item
          })
        }
      />
    </>
  );
};

function _copyAndSort<T>(
  items: T[],
  columnKey: string,
  isSortedDescending?: boolean
): T[] {
  const key = columnKey as keyof T;
  return items
    .slice(0)
    .sort((a: T, b: T) =>
      (isSortedDescending ? a[key] < b[key] : a[key] > b[key]) ? 1 : -1
    );
}
