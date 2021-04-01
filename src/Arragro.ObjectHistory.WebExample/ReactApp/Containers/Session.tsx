import * as React from 'react'
import { Link } from 'react-router-dom'
import { useParams } from 'react-router'
import { Formik, Form, FormikHelpers } from 'formik'
import * as Yup from 'yup'
import * as dayjs from 'dayjs'

import { utils } from '../../ReactAppLibrary'
import TextBox from '../FormikControls/TextBox'

const { HttpUtils } = utils
export interface IDrillData {
    id: string
    name: string
    description: string
    dateCreated: Date
    duration: number
}

export interface ISessionDrillContainer {
    id: number
    name: string
    dateCreated: Date
    drills: Array<IDrillData>
    objectHistoryKey: string
}

export interface IDrillForm {
    name: string
    description: string
    duration: number
}

const SessionPage = () => {
    const params = useParams<{ id: string }>()

    const [sessionDrillContainer, setSessionDrillContainer] = React.useState<ISessionDrillContainer | null>(null)

    React.useEffect(() => {
        utils.HttpUtils.get<ISessionDrillContainer>(`/api/drills/forsession/${params.id}`)
            .then((response) => {
                if (response.ok) {
                    setSessionDrillContainer(response.data)
                } else {
                    alert('Something went wrong getting drill data')
                }
            })
            .catch(ex => {
                console.log(ex)
            })
    }, [])

    const getDrills = () => {
        if (sessionDrillContainer === null) {
            return null
        }

        const output: Array<JSX.Element> = []

        for (let i = 0; i < sessionDrillContainer.drills.length; i++) {
            const drill = sessionDrillContainer.drills[i]

            output.push(
                <div key={drill.id} className='panel panel-default'>
                    <div className='panel-heading'>
                        {drill.name}
                    </div>
                    <div className='panel-body'>
                        {drill.description}
                    </div>
                </div>)
        }

        return output
    }

    const getInitialValues = (): IDrillForm => {
        return {
            name: '',
            description: '',
            duration: 10
        }
    }

    const submitForm = (drill: IDrillForm, formikHelpers: FormikHelpers<IDrillForm>) => {
        const drillPostData = {
            sessionId: sessionDrillContainer!.id,
            ...drill
        }
        HttpUtils.post<IDrillForm, ISessionDrillContainer>('/api/drills/create', drillPostData)
            .then((response) => {
                if (response.ok) {
                    setSessionDrillContainer(response.data)
                    formikHelpers.resetForm({ values: getInitialValues() })
                } else {
                    formikHelpers.setSubmitting(false)
                    alert('Something went wrong posting drill data')
                }
            })
            .catch(ex => {
                console.log(ex)
            })
    }


    if (sessionDrillContainer === null) {
        return null
    }

    return <>
        <h2>Training Session: {sessionDrillContainer.name} - <Link to={`/arragro-object-history/${sessionDrillContainer.objectHistoryKey}`}>History</Link></h2>
        <div className='small'>{dayjs.utc(sessionDrillContainer.dateCreated).local().format('DD/MM/YYYY h:mm:ss a')}</div>
        <div className='row'>
            <div className='col-md-9'>
                <h3>Drill Count: {sessionDrillContainer.drills.length}</h3>
                <div>
                    {getDrills()}
                </div>
            </div>
            <div className='col-md-3'>
                <div className='panel panel-primary'>
                    <div className='panel-heading'>
                        Add New Drill
                    </div>
                    <div className='panel-body'>
                        <Formik
                            initialValues={getInitialValues()}
                            onSubmit={(values, formikHelpers) => {
                                submitForm(values, formikHelpers)
                            }}
                            validationSchema={Yup.object().shape({
                                name: Yup.string()
                                    .required('Please supply Name')
                                    .max(20, 'There is a 20 character limit to this field.'),
                                description: Yup.string()
                                    .required('Please supply Description')
                                    .max(50, 'There is a 50 character limit to this field.'),
                                duration: Yup.number()
                                    .required('Please supply Duration')
                            })}>
                            {({ submitCount, handleBlur, handleChange, values, errors, isSubmitting }) => (
                                <Form className={submitCount > 0 ? 'was-validated' : ''}>
                                    <TextBox
                                        type='text'
                                        label='Name'
                                        id='name'
                                        submitCount={submitCount}
                                        value={values.name}
                                        error={errors.name}
                                        handleChange={handleChange}
                                        handleBlur={handleBlur}
                                    />

                                    <TextBox
                                        type='text'
                                        label='Description'
                                        id='description'
                                        submitCount={submitCount}
                                        value={values.description}
                                        error={errors.description}
                                        handleChange={handleChange}
                                        handleBlur={handleBlur}
                                    />

                                    <TextBox
                                        type='number'
                                        label='Duration'
                                        id='duration'
                                        submitCount={submitCount}
                                        value={values.duration.toString()}
                                        error={errors.duration}
                                        handleChange={handleChange}
                                        handleBlur={handleBlur}
                                    />
                                    <button type='submit' className='btn btn-primary' disabled={isSubmitting}>Add Drill</button>
                                </Form>
                            )}
                        </Formik>
                    </div>
                </div>
            </div>
        </div>
        <div className='row'>
            <div className='col-md-12'>
                <a href='/'>Return home</a>
            </div>
        </div>
    </>
}

export default SessionPage
