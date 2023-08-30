import React, { useContext, type FunctionComponent, useCallback, type FormEvent, useRef } from 'react'

import { Authenticator } from 'src/components'
import BasedGhostLogoPNG from '../../assets/image/based-ghost-main.png'
import { useAppDispatch, useAppSelector } from '../../store'
import { Routes } from '../../config'
import { toast, type Id } from 'react-toastify'
import alerttoast from 'react-hot-toast'
import { useNavigate } from 'react-router-dom'
import LoginContext from '../../context/LoginContext'
import { loginAsync, setAuthStatus, resetState, AuthStatusEnum, type Credentials } from '../../store/authSlice'
import { Button, Checkbox, Input } from '@material-tailwind/react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

const LoginForm: FunctionComponent = () => {
  const toastIdRef = useRef<Id>('')
  const {
    userName,
    setUserName,
    password,
    setPassword,
    newPassword,
    setNewPassword,
    toggleShow,
    rememberMe,
    show,
    isInvalid,
    setInvalid,
    setRememberMe,
  } = useContext(LoginContext)
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const status = useAppSelector<AuthStatusEnum>((state) => state.auth.status)
  const error = useAppSelector<string | undefined>((state) => state.auth.error)
  const newShow = useAppSelector<boolean>((state) => state.auth.needNew)
  const dispatchAuthStatus = useCallback(
    (status: AuthStatusEnum): void => {
      dispatch(setAuthStatus(status))
    },
    [dispatch],
  )
  const onFailedAuth = useCallback(
    (error: any): void => {
      alerttoast.error(`login fails with ${error}`)

      dispatchAuthStatus(AuthStatusEnum.NONE)
      dispatch(resetState())
      setInvalid && setInvalid(true)
    },
    [dispatch, dispatchAuthStatus],
  )
  const onSuccessfulAuth = useCallback((): void => {
    const homePath = Routes.find((x) => x.name === 'Home')?.path ?? '/'
    navigate(homePath)
  }, [navigate])

  const handleLogin = (e: FormEvent<HTMLFormElement>): void => {
    e.preventDefault()
    if (status === AuthStatusEnum.PROCESS) {
      return
    }

    if (!userName || !password || (newShow && !newPassword)) {
      // Run invalidInputs error and display toast notification (if one is not already active)
      setInvalid && setInvalid(true)
      if (!toast.isActive(toastIdRef.current)) {
        const plusnew = newShow ? ' and new password' : ''

        toastIdRef.current = toast.error(`Enter user name/password${plusnew}`)
      }
    } else {
      // Clear any toast notifications and prepare state for Login request stub / run login request stub
      toast.dismiss()
      setInvalid && setInvalid(false)
      dispatchAuthStatus(AuthStatusEnum.PROCESS)

      setTimeout(() => {
        const credentials: Credentials = {
          rememberMe,
          userName: userName,
          password: password,
          newPassword: newPassword,
        }

        dispatch(loginAsync(credentials))
      }, 2000)
    }
  }

  return (
    <section className='section section-login'>
      <div className='container has-text-centered'>
        <div className='column is-4 is-offset-4'>
          <h3 className='title'>Login</h3>
          <p className='subtitle'>Please login to proceed</p>
          <div className='box login-box'>
            <img className='block ml-[auto] mr-[auto]' width='170' aria-hidden id='login-img' alt='based-ghost-logo' src={BasedGhostLogoPNG} />
            <form onSubmit={handleLogin}>
              <div className='mb-4 flex flex-col gap-2'>
                <Input
                  variant='outlined'
                  label='User Name'
                  type='text'
                  crossOrigin
                  value={userName}
                  error={isInvalid}
                  onChange={(e) => setUserName && setUserName(e.target.value)}
                />
                <Input
                  variant='outlined'
                  label='Password'
                  type={show ? 'password' : 'text'}
                  crossOrigin
                  value={password}
                  error={isInvalid}
                  onChange={(e) => setPassword && setPassword(e.target.value)}
                  icon={show ? <FontAwesomeIcon icon='eye' onClick={toggleShow} /> : <FontAwesomeIcon icon='eye-slash' onClick={toggleShow} />}
                />
                {newShow ? (
                  <Input
                    variant='outlined'
                    label='New Password'
                    type={show ? 'password' : 'text'}
                    crossOrigin
                    value={newPassword}
                    error={isInvalid}
                    onChange={(e) => setNewPassword && setNewPassword(e.target.value)}
                    icon={show ? <FontAwesomeIcon icon='eye' onClick={toggleShow} /> : <FontAwesomeIcon icon='eye-slash' onClick={toggleShow} />}
                  />
                ) : (
                  <></>
                )}
                <Checkbox
                  label='Remember Me'
                  checked={rememberMe}
                  crossOrigin
                  ripple={false}
                  onChange={() => setRememberMe && setRememberMe(!rememberMe)}
                />
              </div>

              <Button type='submit' color='green' fullWidth>
                Login{" "}
                <FontAwesomeIcon icon='sign-in-alt' />
              </Button>
              <Authenticator error={error} authStatus={status} handleOnFail={onFailedAuth} handleOnSuccess={onSuccessfulAuth} />
            </form>
          </div>
        </div>
      </div>
    </section>
  )
}
LoginForm.displayName = 'LoginForm'

export default LoginForm
