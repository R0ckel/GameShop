import styles from "../../../css/app.module.css";
import {useState} from "react";
import styled from "styled-components";

import {Field, Form, Formik} from 'formik';
import * as Yup from 'yup';
import {Button, Input, message, Modal} from 'antd';
import {useDispatch, useSelector} from "react-redux";
import {AuthService} from "../../../services/authService";
import {Link, useNavigate} from "react-router-dom";
import {setUserData} from "../../../context/store";

const NameSpan = styled.span`
  margin: 0;
  color: lightblue;
`

const StatusButton = styled.button`
  background-color: inherit;
  padding: 14px 28px;
  font-size: 16px;
  cursor: pointer;
  display: inline-block;
  border-radius: 1vh;
  color: white;

  &:hover {
    background: rgba(255, 255, 255, 0.3);
  }

  &:active {
    background: rgba(255, 255, 255, 0.2);
  }
`

const StyledStatusButton = styled(StatusButton)`
  border: ${props => props.loggedIn ? '1px dotted crimson' : '3px double white'};
  margin-left: 10px;
`

const MarginedLabel = styled("label")`
  margin: 10px 0 2px;
`;

const MarginedButton = styled(Button)`
  margin-top: 20px;
`;

const Error = styled("div")`
  color: red;
`;

const LoginSchema = Yup.object().shape({
  email: Yup.string()
  .required('Please enter your email!'),
  password: Yup.string()
  .required('Please enter your password!')
});

const RegistrationSchema = Yup.object().shape({
  name: Yup.string()
  .required('Please enter your name!')
  .max(40, 'First name must be at most 40 characters long'),

  surname: Yup.string()
  .required('Please enter your surname!')
  .max(40, 'Last name must be at most 40 characters long'),

  email: Yup.string()
  .required('Please enter your email!')
  .email('Invalid email address'),

  birthDate: Yup.date()
  .required('Please, enter your birth date!'),

  newPassword: Yup.string()
  .matches(
    /^(?=.*[A-Z])(?=.*[^a-zA-Z])/,
    'Password must contain at least one uppercase letter and one non-letter character'
  )
  .matches(/\d/, 'Password must contain at least one digit')
  .min(8, 'Password must be at least 8 characters long')
  .max(64, 'Password must be at most 64 characters long')
  .required('Enter your password!'),

  confirmPassword: Yup.string()
  .oneOf([Yup.ref('newPassword')], 'Passwords must match')
  .required('Confirm your password!'),
});

const LoginPanel = () => {
  const {isLoggedIn, username} = useSelector(state => state.userData);
  const dispatch = useDispatch()
  const [isLoginModalVisible, setIsLoginModalVisible] = useState(false);
  const [isRegistrationModalVisible, setIsRegistrationModalVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const navigate  = useNavigate();

  async function handleLogOut() {
    const response = await AuthService.logout()
    if (response.success){
      dispatch(setUserData({isLoggedIn: false}))
      navigate('/');
    } else {
      message.error("Error while logout!")
    }
  }

  const handleLogin = async (values) => {
    setIsLoading(true);
    try {
      const response = await AuthService.login(values);
      console.log(response)
      if (response.success) {
        // Login successful
        const userData = AuthService.getUserClaims();
        AuthService.applyUserDataToContext(userData, dispatch);

        setIsLoginModalVisible(false);
        message.success('Login successful');
      } else {
        // Login failed
        message.error(response.message).then(() => {
          message.error("Email or password may be incorrect")
        });
      }
    } catch (error) {
      // Handle error
      message.error(error)
    }
    setIsLoading(false);
  };

  const handleRegistration = async (values) => {
    setIsLoading(true);
    try {
      const response = await AuthService.register(values);
      console.log(response)
      if (response.success) {
        // Registration successful
        message.success('Registration successful!');
        // Login instantly
        const email = values.email;
        const password = values.password;
        await handleLogin({email, password})

        setIsRegistrationModalVisible(false);
      } else {
        // Registration failed
        message.error(response.message);
      }
    } catch (error) {
      // Handle error
      message.error(error)
    }
    setIsLoading(false);
  };

  const showLoginModal = () => {
    setIsLoginModalVisible(true);
  };

  const showRegistrationModal = () => {
    setIsRegistrationModalVisible(true);
  };

  const handleCancel = () => {
    setIsLoginModalVisible(false);
    setIsRegistrationModalVisible(false);
  };

  return (
    <div className={`${styles.aright}`}>
      {isLoggedIn ?
        <div>
          <NameSpan>
            <Link to={`/profile`} className={`${styles.noLink}`} >
              <button className={`${styles.btn} ${styles.info}`}>
                {username}
              </button>
            </Link>
          </NameSpan>
          <StyledStatusButton loggedIn onClick={handleLogOut}>
            Log out
          </StyledStatusButton>
        </div>
        :
        <>
          <StyledStatusButton onClick={showLoginModal}>
            Log in
          </StyledStatusButton>
          <StyledStatusButton onClick={showRegistrationModal}>
            Register
          </StyledStatusButton>

          <Modal title="Log in" open={isLoginModalVisible} onCancel={handleCancel} footer={null}>
            <div style={{display: 'flex', justifyContent: 'center', overflow: 'auto'}}>
              <Formik
                initialValues={{email: '', password: ''}}
                validationSchema={LoginSchema}
                onSubmit={handleLogin}
              >
                {({errors, touched}) => (
                  <Form style={{width: '100%'}}>
                    <MarginedLabel htmlFor="email">Email</MarginedLabel>
                    <Field id="email" name="email" as={Input} placeholder="example@mail.com" autoComplete="email"/>
                    {errors.email && touched.email ? (
                      <Error>{errors.email}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="password">Password</MarginedLabel>
                    <Field id="password" name="password" as={Input.Password} placeholder="Password" autoComplete="current-password"/>
                    {errors.password && touched.password ? (
                      <Error>{errors.password}</Error>
                    ) : null}

                    <MarginedButton type="primary" htmlType="submit" loading={isLoading}>
                      Log in
                    </MarginedButton>
                  </Form>
                )}
              </Formik>
            </div>
          </Modal>

          <Modal title="Register" open={isRegistrationModalVisible} onCancel={handleCancel} footer={null}>
            <div style={{display: 'flex', justifyContent: 'center', overflow: 'auto'}}>
              <Formik
                initialValues={{
                  name: '',
                  surname: '',
                  email: '',
                  birthDate: '',
                  password: '',
                  confirmPassword: ''
                }}
                validationSchema={RegistrationSchema}
                onSubmit={handleRegistration}
              >
                {({errors, touched}) => (
                  <Form style={{width: '100%'}}>
                    <MarginedLabel htmlFor="name">Name</MarginedLabel>
                    <Field id="name" name="name" as={Input} placeholder="Name" autoComplete="given-name"/>
                    {errors.name && touched.name ? (
                      <Error>{errors.name}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="surname">Surname</MarginedLabel>
                    <Field id="surname" name="surname" as={Input} placeholder="Surname" autoComplete="family-name"/>
                    {errors.surname && touched.surname ? (
                      <Error>{errors.surname}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="email">Email</MarginedLabel>
                    <Field id="email" name="email" type="email" as={Input} placeholder="Email" autoComplete="email"/>
                    {errors.email && touched.email ? (
                      <Error>{errors.email}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="birthDate">Birth Date</MarginedLabel>
                    <Field id="birthDate" name="birthDate" type="date" />
                    {errors.birthDate && touched.birthDate ? (
                      <Error>{errors.birthDate}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="password">Password</MarginedLabel>
                    <Field id="password" name="password" as={Input.Password} placeholder="Password" autoComplete="new-password"/>
                    {errors.password && touched.password ? (
                      <Error>{errors.password}</Error>
                    ) : null}

                    <MarginedLabel htmlFor="confirmPassword">Confirm Password</MarginedLabel>
                    <Field id="confirmPassword" name="confirmPassword" as={Input.Password} placeholder="Confirm Password"
                           autoComplete="new-password"/>
                    {errors.confirmPassword && touched.confirmPassword ? (
                      <Error>{errors.confirmPassword}</Error>
                    ) : null}

                    <MarginedButton type="primary" htmlType="submit" loading={isLoading}>
                      Register
                    </MarginedButton>
                  </Form>
                )}
              </Formik>
            </div>
          </Modal>
        </>
      }
    </div>
  )
}

export default LoginPanel;