import * as Yup from "yup";

export const CreateSchema = Yup.object().shape({
  phrase: Yup.string()
    .required("black country phrase is required")
    .test("len", "", val => val.toString().length < 140),
  translation: Yup.string()
    .required("how will anyone understand you?")
    .test("len", "", val => val.toString().length < 140)
});

export const EditSchema = Yup.object().shape({
  phrase: Yup.string()
    .required("black country phrase is required")
    .test("len", "", val => val.toString().length < 140),
  translation: Yup.string()
    .required("how will anyone understand you?")
    .test("len", "", val => val.toString().length < 140)
});
