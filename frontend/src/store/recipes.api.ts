import axios from 'axios';
import { Recipe } from '../types';

const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000/';

export interface TRecipesResponse {
    data: Recipe[];
    entitiesCount: number;
    pagesCount: number;
}

// Получение списка рецептов
export const getRecipesApi = async (): Promise<Recipe[]> => {
    try {
        const response = await axios.get<TRecipesResponse>(
            `${apiUrl}api/receipts`,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );

        return response.data.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting recipes going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Удаление рецепта
export const deleteRecipeApi = async (id: string): Promise<string> => {
    try {
        const response = await axios.delete(`${apiUrl}api/receipts/${id}`, {
            headers: {
                Accept: '*/*',
            },
        });

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Deleting recipe failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Загрузка фотографии рецепта
export const uploadRecipeImage = async (
    file?: File
): Promise<{ url: string }> => {
    try {
        if (!file) {
            return { url: '' };
        }

        const formData = new FormData();
        formData.append('Files', file);

        const response = await axios.post<{ url: string }[]>(
            `${apiUrl}api/files/receipts`,
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Accept: '*/*',
                },
            }
        );

        return response.data[0];
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Image upload failed');
        }

        throw new Error('Unknown error occurred during image upload');
    }
};

export interface CreateRecipeDto {
    title: string;
    description: string;
    mainPictureUrl?: { url: string };
}

// Добавление нового рецепта
export const createRecipe = async (
    recipe: CreateRecipeDto,
    imageFile?: File
): Promise<string> => {
    try {
        const imageUrl = await uploadRecipeImage(imageFile);

        let recipeWithImage: CreateRecipeDto = {
            ...recipe,
        };

        if (imageUrl) {
            recipeWithImage = {
                ...recipeWithImage,
                mainPictureUrl: imageUrl,
            };
        }

        const response = await axios.post<string>(
            `${apiUrl}api/receipts`,
            recipeWithImage,
            {
                headers: {
                    'Content-Type': 'application/json-patch+json',
                    Accept: '*/*',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Recipe creation failed'
            );
        }
        throw new Error('Unknown error during recipe creation');
    }
};

//Редактирование рецепта
export type UpdateRecipeDto = {
    title?: string;
    description?: string;
    mainPictureUrl?: { url: string };
};

export const updateRecipeApi = async (
    id: string,
    data: UpdateRecipeDto,
    imageFile?: File
): Promise<void> => {
    try {
        const imageUrl = await uploadRecipeImage(imageFile);

        let recipeWithImage: UpdateRecipeDto = {
            ...data,
        };

        if (imageUrl) {
            recipeWithImage = {
                ...recipeWithImage,
                mainPictureUrl: imageUrl,
            };
        }

        await axios.put(`${apiUrl}api/receipts/${id}`, recipeWithImage, {
            headers: {
                Accept: '*/*',
                'Content-Type': 'application/json',
            },
        });
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Updating recipe failed');
        }

        throw new Error('Unknown error occurred');
    }
};
