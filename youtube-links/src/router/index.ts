import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../pages/HomePage.vue';
import TestPage from '@/views/TestPage.vue';
import PlaylistsPage from '@/pages/Playlists/PlaylistsPage.vue';
import DownloadLinkPage from '@/pages/Links/DownloadLinkPage.vue';
import UsersPage from '@/pages/Users/UsersPage.vue';
import LinksPage from '@/pages/Links/LinksPage.vue';
import AboutPage from '../views/AboutPage.vue';
import UnauthorizedErrorPage from '@/pages/Error/UnauthorizedErrorPage.vue';
import ForbiddenErrorPage from '@/pages/Error/ForbiddenErrorPage.vue';
import NotFoundErrorPage from '@/pages/Error/NotFoundErrorPage.vue';
import ValidationErrorPage from '@/pages/Error/ValidationErrorPage.vue';
import ServerErrorPage from '@/pages/Error/ServerErrorPage.vue';

export namespace RouteName {
	// Navbar pages
	export const HOME = 'home';
	export const DOWNLOAD_LINK = 'download-link';
	export const USERS = 'users';
	export const ABOUT = 'about';
	export const TEST = 'test';

	export const PLAYLISTS = 'playlists';
	export const LINKS = 'links';

	//Errors
	export const UNAUTHORIZED = 'unauthorized';
	export const FORBIDDEN = 'forbidden';
	export const NOT_FOUND = 'not-found';
	export const VALIDATION_ERROR = 'validation-error';
	export const SERVER_ERROR = 'server-error';
}

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: [
		{
			path: '/',
			name: RouteName.HOME,
			component: HomePage,
		},
		{
			path: `/${RouteName.DOWNLOAD_LINK}`,
			name: RouteName.DOWNLOAD_LINK,
			component: DownloadLinkPage,
		},
		{
			path: `/${RouteName.USERS}`,
			name: RouteName.USERS,
			component: UsersPage,
		},
		{
			path: `/${RouteName.ABOUT}`,
			name: RouteName.ABOUT,
			component: AboutPage,
		},
		{
			path: `/${RouteName.TEST}`,
			name: RouteName.TEST,
			component: TestPage,
		},
		{
			path: `/${RouteName.PLAYLISTS}/:userId`,
			name: RouteName.PLAYLISTS,
			component: PlaylistsPage,
			props: (route) => ({ userId: Number(route.params.userId) }),
		},
		{
			path: `/${RouteName.LINKS}/:userId/:playlistId`,
			name: RouteName.LINKS,
			component: LinksPage,
			props: (route) => ({
				userId: Number(route.params.userId),
				playlistId: Number(route.params.playlistId),
			}),
		},
		{
			path: `/${RouteName.UNAUTHORIZED}`,
			name: RouteName.UNAUTHORIZED,
			component: UnauthorizedErrorPage,
		},
		{
			path: `/${RouteName.FORBIDDEN}`,
			name: RouteName.FORBIDDEN,
			component: ForbiddenErrorPage,
		},
		{
			path: `/${RouteName.NOT_FOUND}`,
			name: RouteName.NOT_FOUND,
			component: NotFoundErrorPage,
		},
		{
			path: `/${RouteName.VALIDATION_ERROR}`,
			name: RouteName.VALIDATION_ERROR,
			component: ValidationErrorPage,
		},
		{
			path: `/${RouteName.SERVER_ERROR}`,
			name: RouteName.SERVER_ERROR,
			component: ServerErrorPage,
		},
	],
});

export default router;
