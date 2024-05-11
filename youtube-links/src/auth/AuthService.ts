import type { JwtDto } from '@/shared/features/users/responses/JwtDto';
import { ref } from 'vue';
import { jwtProvider } from './JwtProvider';

const useAuthService = () => {
  const isAuthenticated = ref(false);

  const getCurrentUserId = (): void => {
    var tokens = jwtProvider.getJwtDto();
    //decode token
    // tokens?.accessToken;
  };

  const isLoggedInUser = (userId: number): void => {};

  const login = (token: JwtDto): void => {
    jwtProvider.setJwtDto(token);
  };

  const logout = (): void => {
    jwtProvider.removeJwtDto();
  };

  const refreshToken = (): void => {};

  return { getCurrentUserId, isLoggedInUser, login, logout, refreshToken };
};

export default useAuthService;
