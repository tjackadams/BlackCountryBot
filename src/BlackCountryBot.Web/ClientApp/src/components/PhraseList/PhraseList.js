import React from "react";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import debounce from "lodash/debounce";
import {
  ConstrainMode,
  ShimmeredDetailsList,
  DetailsListLayoutMode,
  MarqueeSelection,
  mergeStyleSets,
  Selection,
  TextField,
  Text,
  CommandBar,
  mergeStyles,
  getTheme,
  Modal,
  getId,
  DefaultPalette
} from "office-ui-fabric-react";

import { EntryForm } from "../EntryForm";

import { actionCreators } from "../../store/Phrases";

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

class PhraseList extends React.PureComponent {
  state = {
    items: [],
    columns: [
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
        onColumnClick: (ev, column) => this._onColumnClick(ev, column),
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
        onColumnClick: (ev, column) => this._onColumnClick(ev, column),
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
        onColumnClick: (ev, column) => this._onColumnClick(ev, column),
        data: "number",
        onRender: item => {
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
        onColumnClick: (ev, column) => this._onColumnClick(ev, column),
        data: "string",
        isPadded: true,
        onRender: item => {
          return <span>{item.lastTweetTime}</span>;
        }
      }
    ],
    selectionDetails: "No items selected",
    showModal: false,
    selection: new Selection({
      onSelectionChanged: () => {
        this.setState({
          selectionDetails: this._getSelectionDetails()
        });
      }
    }),
    isDataLoaded: false
  };

  componentDidUpdate(prevProps, prevState) {
    if (prevProps.phrases !== this.props.phrases) {
      this.setState({
        items: this.props.phrases,
        isDataLoaded: true
      });
    }
  }

  _getCommandItems = () => {
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

  _getSelectionDetails() {
    const selectionCount = this.state.selection.getSelectedCount();

    switch (selectionCount) {
      case 0:
        return "No items selected";
      case 1:
        return (
          "1 item selected: " + this.state.selection.getSelection()[0].phraseId
        );
      default:
        return `${selectionCount} items selected`;
    }
  }

  _showModal = () => {
    this.setState({ showModal: true });
  };

  _closeModal = ev => {
    if (ev !== undefined) {
      ev.preventDefault();
    }
    this.setState({ showModal: false });
  };

  _onDeleteRow = () => {
    if (this.state.selection.getSelectedCount() > 0) {
      for (const selection of this.state.selection.getSelection()) {
        this.props.delete({ id: selection.phraseId });
      }
    } else {
      this.props.delete({
        id: this.state.selection.getSelection()[0].phraseId
      });
    }
  };

  _onFilter = () =>
    debounce((ev, text) => {
      console.log("filtering: ", text);
      this.setState({
        items: text
          ? this.state.items.filter(
              i =>
                i.original.toLowerCase().indexOf(text) > -1 ||
                i.translation.toLowerCase().indexOf(text) > -1
            )
          : this.props.phrases
      });
    }, 500);

  _onColumnClick = (ev, column) => {
    const { columns, items } = this.state;
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
      currColumn.fieldName,
      currColumn.isSortedDescending
    );
    this.setState({
      columns: newColumns,
      items: newItems
    });
  };

  render() {
    const {
      columns,
      items,
      selectionDetails,
      showModal,
      selection,
      isDataLoaded
    } = this.state;

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
            onChange={this._onFilter()}
            styles={{ root: { maxWidth: "300px" } }}
          />
        </div>
        <MarqueeSelection selection={selection}>
          <ShimmeredDetailsList
            items={items}
            columns={columns}
            isHeaderVisible={true}
            selection={selection}
            selectionPreservedOnEmptyClick={true}
            constrainMode={ConstrainMode.horizontalConstrained}
            layoutMode={DetailsListLayoutMode.justified}
            enableShimmer={!isDataLoaded}
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
            <EntryForm
              onClose={this._closeModal}
              onSubmit={this.props.create}
            />
          </div>
        </Modal>
      </div>
    );
  }
}

function _copyAndSort(items, columnKey, isSortedDescending) {
  const key = columnKey;
  return items
    .slice(0)
    .sort((a, b) =>
      (isSortedDescending ? a[key] < b[key] : a[key] > b[key]) ? 1 : -1
    );
}

export default connect(
  state => state.phrases,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(PhraseList);
