import React, { Component } from "react";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import {
  IColumn,
  ConstrainMode,
  DetailsList,
  DetailsListLayoutMode,
  MarqueeSelection,
  mergeStyleSets,
  Selection,
  TextField,
  Text,
  IContextualMenuItem,
  CommandBar,
  mergeStyles,
  getTheme,
  Modal,
  getId,
  DefaultPalette
} from "office-ui-fabric-react";
import debounce from "lodash/debounce";

import { ApplicationState, ConnectedReduxProps } from "../../store";
import {
  IPhraseListState,
  IPhraseListProps,
  PropsFromState,
  PropsFromDispatch
} from "./PhraseList.types";
import { EntryForm } from "../EntryForm";
import { Phrase } from "../../store/phrases/types";

const theme = getTheme();
const classNames = mergeStyleSets({
  headerDivider: {
    display: "inline-block",
    height: "100%"
  },
  headerDividerBar: {
    display: "none",
    background: theme.palette.themePrimary,
    position: "absolute",
    top: 0,
    bottom: 0,
    width: "1px",
    zIndex: 5
  },

  controlWrapper: {
    display: "flex",
    flexWrap: "wrap"
  },

  selectionDetails: {
    marginBottom: "20px"
  },
  modalContainer: {
    height: "60vh",
    width: "60vw",
    display: "flex",
    flexFlow: "column nowrap",
    alignItems: "stretch"
  },
  modalHeader: {
    flex: "1 1 auto",
    background: theme.palette.themePrimary,
    color: DefaultPalette.white,
    display: "flex",
    alignItems: "center",
    padding: "0 28px",
    minHeight: "40px"
  },
  modalBody: {
    flex: "4 4 auto",
    padding: "5px 28px",
    overflowY: "hidden"
  }
});

const rootClass = mergeStyles({
  selectors: {
    [`.${classNames.headerDivider}:hover + .${classNames.headerDividerBar}`]: {
      display: "inline"
    }
  }
});

type AllProps = PropsFromState & PropsFromDispatch & ConnectedReduxProps;

class PhraseList extends Component<AllProps, IPhraseListState> {
  private _selection: Selection;
  private _allItems: Phrase[];
  private _titleId: string = getId("title");
  private _subtitleId: string = getId("subText");

  constructor(props: AllProps) {
    super(props);

    this._allItems = this.props.data;

    const columns: IColumn[] = [
      {
        key: "column1",
        name: "Phrase",
        fieldName: "original",
        minWidth: 210,
        maxWidth: 400,
        isRowHeader: true,
        isResizable: true,
        isSorted: true,
        isSortedDescending: false,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
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
        onColumnClick: this._onColumnClick,
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
        onColumnClick: this._onColumnClick,
        data: "number",
        onRender: (item: Phrase) => {
          return <span>{item.numberOfTweets}</span>;
        }
      },
      {
        key: "column4",
        name: "Last Tweeted",
        fieldName: "lastTweetTime",
        minWidth: 70,
        maxWidth: 90,
        isResizable: true,
        onColumnClick: this._onColumnClick,
        data: "string",
        isPadded: true,
        onRender: (item: Phrase) => {
          return <span>{item.lastTweetTime}</span>;
        }
      }
    ];

    this._selection = new Selection({
      onSelectionChanged: () => {
        this.setState({
          selectionDetails: this._getSelectionDetails()
        });
      }
    });

    this.state = {
      items: this._allItems,
      columns: columns,
      selectionDetails: this._getSelectionDetails(),
      showModal: false
    };
  }

  componentDidUpdate(prevProps: AllProps, prevState: IPhraseListState) {
    if (prevProps.data !== this.props.data) {
      this._allItems = this.props.data;
      this.setState({ items: this.props.data });
    }
  }

  public render() {
    const { columns, items, selectionDetails, showModal } = this.state;

    return (
      <div className={rootClass}>
        <CommandBar
          styles={{ root: { marginBottom: "40px" } }}
          items={this._getCommandItems()}
          farItems={[{ key: "count", text: selectionDetails }]}
        />
        <div className={classNames.controlWrapper}>
          <TextField
            className={selectionDetails}
            label="Filter:"
            onChange={this._onFilter}
            styles={{ root: { maxWidth: "300px" } }}
          />
        </div>
        <MarqueeSelection selection={this._selection}>
          <DetailsList
            items={items}
            columns={columns}
            getKey={this._getKey}
            setKey="set"
            isHeaderVisible={true}
            selection={this._selection}
            selectionPreservedOnEmptyClick={true}
            constrainMode={ConstrainMode.horizontalConstrained}
            layoutMode={DetailsListLayoutMode.justified}
          />
        </MarqueeSelection>
        <Modal
          titleAriaId={this._titleId}
          subtitleAriaId={this._subtitleId}
          isOpen={showModal}
          onDismiss={this._closeModal}
          containerClassName={classNames.modalContainer}
        >
          <div className={classNames.modalHeader}>
            <Text variant="xxLarge" id={this._titleId}>
              New Phrase
            </Text>
          </div>
          <div id={this._subtitleId} className={classNames.modalBody}>
            <EntryForm onClose={this._closeModal} />
          </div>
        </Modal>
      </div>
    );
  }

  private _showModal = (): void => {
    this.setState({ showModal: true });
  };

  private _closeModal = (ev?: React.MouseEvent<HTMLButtonElement>): void => {
    if (ev !== undefined) {
      ev.preventDefault();
    }
    this.setState({ showModal: false });
  };

  private _getSelectionDetails(): string {
    const selectionCount = this._selection.getSelectedCount();

    switch (selectionCount) {
      case 0:
        return "No items selected";
      case 1:
        return (
          "1 item selected: " +
          (this._selection.getSelection()[0] as Phrase).phraseId
        );
      default:
        return `${selectionCount} items selected`;
    }
  }

  private _getCommandItems = (): IContextualMenuItem[] => {
    return [
      {
        key: "addPhrase",
        text: "New phrase",
        iconProps: { iconName: "Add" },
        onClick: this._showModal
      },
      {
        key: "deletePhrase",
        text: "Delete phrase(s)",
        iconProps: { iconName: "Delete" },
        onClick: this._onDeleteRow
      }
    ];
  };

  private _onDeleteRow = (): void => {
    if (this._selection.getSelectedCount() > 0) {
      this.setState((previousState: IPhraseListState) => {
        return {
          items: previousState.items.filter(
            (item, index) => !this._selection.isIndexSelected(index)
          )
        };
      });
    } else {
      this.setState({
        items: this.state.items.slice(1)
      });
    }
  };

  private _onFilter = debounce(
    (
      ev: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>,
      text?: string
    ): void => {
      this.setState({
        items: text
          ? this.state.items.filter(
              i =>
                i.original.toLowerCase().indexOf(text) > -1 ||
                i.translation.toLowerCase().indexOf(text) > -1
            )
          : this.props.data
      });
    },
    500
  );

  private _getKey(item: any, index?: number): string {
    return item.phraseId;
  }

  private _onColumnClick = (
    ev: React.MouseEvent<HTMLElement>,
    column: IColumn
  ): void => {
    const { columns, items } = this.state;
    const newColumns: IColumn[] = columns.slice();
    const currColumn: IColumn = newColumns.filter(
      currCol => column.key === currCol.key
    )[0];
    newColumns.forEach((newCol: IColumn) => {
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
    this.setState({
      columns: newColumns,
      items: newItems
    });
  };
}

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

const mapStateToProps = ({ phrases }: ApplicationState) => {
  return { data: phrases.data };
};

const mapDispatchToProps = {};

export const PhraseListConnected = connect(
  mapStateToProps
  // mapDispatchToProps
)(PhraseList);
