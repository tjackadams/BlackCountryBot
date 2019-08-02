import React from "react";
import {
  Stack,
  ShimmeredDetailsList,
  ICommandBarItemProps,
  IColumn,
  ConstrainMode,
  SelectionMode,
  IconButton
} from "office-ui-fabric-react";

import { PhraseTableCommandBar } from "./";
import { IPhrase } from "../../store/Phrases";

export class PhraseTable extends React.Component<{
  commandItems: ICommandBarItemProps[];
  tableItems: IPhrase[];
  tableColumns: IColumn[];
  isReady: boolean;
  dispatch: Function;
  openModal: Function;
}> {
  public render(): JSX.Element {
    const onRenderItemColumn = (
      item: IPhrase,
      index?: number,
      column?: IColumn
    ) => {
      if (!column) {
        return;
      }
      const fieldContent = item[column.fieldName as keyof IPhrase];
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
                    onClick: () =>
                      this.props.dispatch({
                        type: "SIGNALR_DELETE_PHRASE",
                        command: { id: item.phraseId }
                      })
                  },
                  {
                    key: "update",
                    text: "Update",
                    onClick: () => this.props.openModal(1)
                  },
                  {
                    key: "tweet",
                    text: "Tweet",
                    onClick: () =>
                      this.props.dispatch({
                        type: "SIGNALR_TWEET_PHRASE",
                        command: { id: item.phraseId }
                      })
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

    return (
      <Stack>
        <Stack.Item>
          <PhraseTableCommandBar items={this.props.commandItems} />
        </Stack.Item>
        <Stack.Item>
          <ShimmeredDetailsList
            items={this.props.tableItems}
            columns={this.props.tableColumns}
            constrainMode={ConstrainMode.horizontalConstrained}
            enableShimmer={!this.props.isReady}
            onRenderItemColumn={onRenderItemColumn}
            enableUpdateAnimations={true}
            selectionMode={SelectionMode.none}
          />
        </Stack.Item>
      </Stack>
    );
  }
}
