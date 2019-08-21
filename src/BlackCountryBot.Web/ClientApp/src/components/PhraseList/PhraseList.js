import React, { Suspense } from "react";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import debounce from "lodash/debounce";
import { format } from "date-fns";
import { CommandBar } from "office-ui-fabric-react/lib/CommandBar";
import { IconButton } from "office-ui-fabric-react/lib/Button";
import {
  ConstrainMode,
  SelectionMode
} from "office-ui-fabric-react/lib/DetailsList";
import { ShimmeredDetailsList } from "office-ui-fabric-react/lib/ShimmeredDetailsList";
import {
  getTheme,
  mergeStyles,
  mergeStyleSets
} from "office-ui-fabric-react/lib/Styling";

import { actionCreators } from "../../store/Phrases";

const CreateModal = React.lazy(() => import("./CreateModal"));
const EditModal = React.lazy(() => import("./EditModal"));

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
        minWidth: 300,
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
        minWidth: 120,
        maxWidth: 300,
        isResizable: true,
        onColumnClick: (ev, column) => this._onColumnClick(ev, column),
        data: "string",
        isPadded: true,
        onRender: item => {
          return (
            <span>
              {item.lastTweetTime &&
                format(new Date(item.lastTweetTime), "dd/MM/yy h:mm bbbb")}
            </span>
          );
        }
      }
    ],
    showCreateModal: false,
    showEditModal: false,
    isDataLoaded: false,
    selectedPhrase: null
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
        onClick: this._showCreateModal
      }
    ];
  };

  _showCreateModal = () => {
    this.setState({ showCreateModal: true });
  };

  _showEditModal = () => {
    this.setState({ showEditModal: true });
  };

  _closeModal = ev => {
    if (ev !== undefined) {
      ev.preventDefault();
    }
    this.setState({
      showCreateModal: false,
      showEditModal: false,
      selectedPhrase: null
    });
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

  _deleteItem = (item, index) => {
    this.props.delete({
      id: item.phraseId
    });
  };

  _updateItem = (item, index) => {
    this.setState({
      showEditModal: true,
      selectedPhrase: item
    });
  };

  _onRenderItemColumn = (item, index, column) => {
    const fieldContent = item[column.fieldName];
    if (column.key === "column1") {
      return (
        <div>
          {fieldContent}
          <IconButton
            menuIconProps={{ iconName: "MoreVertical" }}
            role="button"
            aria-haspopup={true}
            aria-label="Show Actions"
            styles={{ root: { float: "right", height: "inherit " } }}
            menuProps={{
              items: [
                {
                  key: "delete",
                  text: "Delete",
                  onClick: () => this._deleteItem(item, index)
                },
                {
                  key: "update",
                  text: "Update",
                  onClick: () => this._updateItem(item, index)
                },
                {
                  key: "tweet",
                  text: "Tweet",
                  onClick: () => this.props.tweet({ id: item.phraseId })
                }
              ]
            }}
          />
        </div>
      );
    } else {
      return <span>{fieldContent}</span>;
    }
  };

  render() {
    const {
      columns,
      items,
      showCreateModal,
      showEditModal,
      isDataLoaded,
      selectedPhrase
    } = this.state;

    return (
      <div className={rootClass}>
        <CommandBar
          styles={{ root: { marginBottom: "40px" } }}
          items={this._getCommandItems()}
        />
        <ShimmeredDetailsList
          items={items}
          columns={columns}
          constrainMode={ConstrainMode.horizontalConstrained}
          enableShimmer={!isDataLoaded}
          onRenderItemColumn={this._onRenderItemColumn}
          enableUpdateAnimations={true}
          selectionMode={SelectionMode.none}
        />
        <Suspense fallback={<div>Loading...</div>}>
          <CreateModal
            show={showCreateModal}
            close={this._closeModal}
            submit={this.props.create}
          />
          <EditModal
            show={showEditModal}
            close={this._closeModal}
            submit={this.props.update}
            phrase={selectedPhrase}
          />
        </Suspense>
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
