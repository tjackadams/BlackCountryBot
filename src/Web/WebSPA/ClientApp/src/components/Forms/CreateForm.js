import React from "react";

import { Formik, Field } from "formik";
import {
  CompoundButton,
  DefaultButton
} from "office-ui-fabric-react/lib/Button";
import { Stack } from "office-ui-fabric-react/lib/Stack";
import { FormikTextField } from "formik-office-ui-fabric-react";

import { CreateSchema } from "./Schema";

export const CreateForm = props => {
  const columnProps = {
    tokens: { childrenGap: 20 }
  };
  const buttonStyle = {
    root: {
      float: "right"
    }
  };

  const _onClose = () => {
    props.onClose();
  };

 
  return (
    <Formik
      validationSchema={CreateSchema}
      onSubmit={(values, actions) => {
        props.onSubmit({
          original: values.phrase,
          translation: values.translation
        });
        actions.setSubmitting(false);
        props.onClose();
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
