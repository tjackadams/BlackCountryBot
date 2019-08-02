import React from "react";

import { Formik, Field } from "formik";
import {
  DefaultButton,
  PrimaryButton
} from "office-ui-fabric-react/lib/Button";
import { Stack } from "office-ui-fabric-react/lib/Stack";
import { FormikTextField } from "formik-office-ui-fabric-react";

import { EditSchema } from "./Schema";

export const EditForm = props => {
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
    <div>
      {props.phrase && (
        <Formik
          initialValues={{
            phraseId: props.phrase.phraseId,
            phrase: props.phrase.original,
            translation: props.phrase.translation
          }}
          validationSchema={EditSchema}
          onSubmit={(values, actions) => {
            props.onSubmit({
              id: values.phraseId,
              original: values.phrase,
              translation: values.translation
            });
            actions.setSubmitting(false);
            props.onClose();
          }}
          render={({ ...props }) => (
            <div>
              <form onSubmit={props.handleSubmit}>
                <Field name="phraseId" required hidden />
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
                      <DefaultButton type="button" onClick={() => _onClose()}>
                        Cancel
                      </DefaultButton>
                      <PrimaryButton
                        type="submit"
                        disabled={
                          props.isSubmitting || !props.dirty || !props.isValid
                        }
                        styles={buttonStyle}
                      >
                        Update
                      </PrimaryButton>
                    </div>
                  </Stack>
                </Stack>
              </form>
            </div>
          )}
        />
      )}
    </div>
  );
};
