import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../pages/HomePage.vue';
import TestPage from '@/views/TestPage.vue';
import PlaylistsPage from '@/pages/Playlists/PlaylistsPage.vue';
import DownloadLinkPage from '@/pages/Links/DownloadLinkPage.vue';
import UsersPage from '@/pages/Users/UsersPage.vue';
import AboutPage from '../views/AboutPage.vue';
import UnauthorizedErrorPage from '@/pages/Error/UnauthorizedErrorPage.vue';
import ForbiddenErrorPage from '@/pages/Error/ForbiddenErrorPage.vue';
import NotFoundErrorPage from '@/pages/Error/NotFoundErrorPage.vue';
import ValidationErrorPage from '@/pages/Error/ValidationErrorPage.vue';
import ServerErrorPage from '@/pages/Error/ServerErrorPage.vue';

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: [
		{
			path: '/',
			name: 'home',
			component: HomePage,
		},
		{
			path: '/download-link',
			name: 'download-link',
			component: DownloadLinkPage,
		},
		{
			path: '/users',
			name: 'users',
			component: UsersPage,
		},
		{
			path: '/my-playlists',
			name: 'my-playlists',
			component: PlaylistsPage,
		},
		{
			path: '/about',
			name: 'about',
			component: AboutPage,
		},
		{
			path: '/test',
			name: 'test',
			component: TestPage,
		},
		{
			path: '/error/unauthorized',
			name: 'unauthorized',
			component: UnauthorizedErrorPage,
		},
		{
			path: '/error/forbidden',
			name: 'forbidden',
			component: ForbiddenErrorPage,
		},
		{
			path: '/error/notfound',
			name: 'notfound',
			component: NotFoundErrorPage,
		},
		{
			path: '/error/validation',
			name: 'validation',
			component: ValidationErrorPage,
		},
		{
			path: '/error/server',
			name: 'server',
			component: ServerErrorPage,
		},
	],
});

export default router;
