import * as React from "react";

import { Formik, Field } from "formik";
import {
  CompoundButton,
  IButtonStyles,
  Stack,
  IStackProps,
  DefaultButton
} from "office-ui-fabric-react";
import { FormikTextField } from "formik-office-ui-fabric-react";

class Values {
  phrase = "";
  translation = "";

  static validate(values: Values) {
    const errors: any = {};

    if (!values.phrase) {
      errors.phrase = "black country phrase is required";
    }

    if (!values.translation) {
      errors.translation = "how will anyone understand you?";
    }

    return errors;
  }
}

export interface IEntryFormProps {
  onClose(): any;
}

export const EntryForm: React.FC<IEntryFormProps> = props => {
  const columnProps: Partial<IStackProps> = {
    tokens: { childrenGap: 20 }
  };
  const buttonStyle: IButtonStyles = {
    root: {
      float: "right"
    }
  };

  const _onClose = () => {
    props.onClose();
  };
  return (
    <Formik
      initialValues={new Values()}
      validate={Values.validate}
      onSubmit={(values, actions) => {
        alert(JSON.stringify(values, null, 2));
        actions.setSubmitting(false);
      }}
      render={({ ...props }) => (
        <div>
          <form onSubmit={props.handleSubmit}>
            <Stack styles={{ root: { width: 400 } }}>
              <Stack {...columnProps}>
                <Field
                  name="phrase"
                  label="Phrase"
                  required
                  component={FormikTextField}
                  multiline
                  autoAdjustHeight
                />
                <Field
                  name="translation"
                  label="Translation"
                  required
                  component={FormikTextField}
                  multiline
                  autoAdjustHeight
                />
                <div>
                  <DefaultButton type="button" onClick={_onClose}>
                    Cancel
                  </DefaultButton>
                  <CompoundButton
                    primary={true}
                    type="submit"
                    disabled={
                      props.isSubmitting || !props.dirty || !props.isValid
                    }
                    secondaryText="Help educate the world."
                    styles={buttonStyle}
                  >
                    Create phrase
                  </CompoundButton>
                </div>
              </Stack>
            </Stack>
          </form>
        </div>
      )}
    />
  );
};
