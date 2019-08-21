import React from "react";
import {
  getTheme,
  mergeStyleSets,
  DefaultPalette
} from "office-ui-fabric-react/lib/Styling";
import { Modal } from "office-ui-fabric-react/lib/Modal";
import { Text } from "office-ui-fabric-react/lib/Text";

import { EditForm } from "../Forms";

const theme = getTheme();
const classNames = mergeStyleSets({
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

export interface IEditModelProps {
  show: boolean;
  close: (ev?: React.MouseEvent<HTMLButtonElement, MouseEvent>) => any;
  submit: any;
  phrase: any;
}

const EditModel = (props: IEditModelProps) => {
  return (
    <Modal
      isOpen={props.show}
      onDismiss={props.close}
      containerClassName={classNames.modalContainer}
    >
      <div className={classNames.modalHeader}>
        <Text variant="xxLarge">Update Phrase</Text>
      </div>
      <div className={classNames.modalBody}>
        <EditForm
          onClose={props.close}
          phrase={props.phrase}
          onSubmit={props.submit}
        />
      </div>
    </Modal>
  );
};

export { EditModel as default };
