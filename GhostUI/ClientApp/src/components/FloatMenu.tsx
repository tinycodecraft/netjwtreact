import React, { useContext } from 'react'
import { StyledSettings,SettingsLink,SettingsMenu, SettingsMenuLink, SettingsMenuTitle,BigIcon } from '../fragments'
import OpenMenuContext from '../context/OpenMenuContext'
import { Routes, NUGET_URL_CONFIG, LINK_ATTRIBUTES } from '../config';
import { AuthApi } from '../api';
import { useAppDispatch } from '../store';
import { useNavigate } from 'react-router-dom';
import { resetState } from '../store/authSlice';
import { useIsLoggedIn } from '../hooks';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


export const FloatMenu = () => {
    
    console.log('float menu rerendering');
    const isLoggedIn = useIsLoggedIn();

    const { MenuOpened,OpenMenu,ref }= useContext(OpenMenuContext)
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
  
    if (!isLoggedIn) {
      return null;
    }
  
    const rootroute  = Routes.find((x) => x.path === '/');
  
    const handleLogout = async () => {
      try {
        await AuthApi.logoutAsync();
        dispatch(resetState());
        navigate(rootroute?.path ?? '/');
      } catch (e) {
        console.error(e);
      }
    };

  return (
    <StyledSettings isMenuOpen={MenuOpened}>
      <SettingsLink
        role="button"
        ref={ref}
        onClick={() => OpenMenu && OpenMenu((prevIsMenuOpen) => !prevIsMenuOpen)}
      >
        <BigIcon icon="cog" />
      </SettingsLink>
      {MenuOpened && (
        <SettingsMenu>
          <SettingsMenuTitle>
            Settings
          </SettingsMenuTitle>
          <li>
            <SettingsMenuLink
              {...LINK_ATTRIBUTES}
              href={NUGET_URL_CONFIG.HealthUi}
            >
              <FontAwesomeIcon icon="heart" /> Health Checks
            </SettingsMenuLink>
          </li>
          <li>
            <SettingsMenuLink
              {...LINK_ATTRIBUTES}
              href={NUGET_URL_CONFIG.SwaggerDocs}
            >
              <FontAwesomeIcon icon="file" /> Swagger API
            </SettingsMenuLink>
          </li>
          <li>
            <SettingsMenuLink
              role="button"
              onClick={handleLogout}
            >
              <FontAwesomeIcon icon={rootroute?.icon ?? 'note'} />{` ${rootroute?.name}`}
            </SettingsMenuLink>
          </li>
        </SettingsMenu>
      )}

    </StyledSettings>
  )
}
