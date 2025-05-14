import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { Recipe } from '../types';
import {
    createRecipe,
    CreateRecipeDto,
    deleteRecipeApi,
    getRecipesApi,
    updateRecipeApi,
    UpdateRecipeDto,
} from './recipes.api';

type RecipesState = {
    recipes: Recipe[];
    error: string | null | undefined;
    isLoading: boolean;
};

export const initialState: RecipesState = {
    recipes: [],
    error: null,
    isLoading: false,
};

export const fetchRecipes = createAsyncThunk(
    'recipes/fetchRecipes',
    async (_, { rejectWithValue }) => {
        try {
            const data = await getRecipesApi();
            return data.recipes;
        } catch (err) {
            return rejectWithValue((err as Error).message);
        }
    }
);

export const deleteRecipe = createAsyncThunk(
    'recipes/deleteRecipe',
    async (id: string, { rejectWithValue }) => {
        try {
            await deleteRecipeApi(id);
            return id;
        } catch (err) {
            return rejectWithValue((err as Error).message);
        }
    }
);

export const addRecipe = createAsyncThunk(
    'recipes/addRecipe',
    async (
        { dto, imageFile }: { dto: CreateRecipeDto; imageFile?: File },
        { rejectWithValue }
    ) => {
        try {
            await createRecipe(dto, imageFile);
            const data = await getRecipesApi();
            return data.recipes;
        } catch (err) {
            return rejectWithValue((err as Error).message);
        }
    }
);

export const updateRecipe = createAsyncThunk(
    'recipes/updateRecipe',
    async (
        {
            id,
            dto,
            imageFile,
        }: { id: string; dto: UpdateRecipeDto; imageFile?: File },
        { rejectWithValue }
    ) => {
        try {
            await updateRecipeApi(id, dto, imageFile);
            const data = await getRecipesApi();
            return data.recipes;
        } catch (err) {
            return rejectWithValue((err as Error).message);
        }
    }
);

export const RecipesSlice = createSlice({
    name: 'recipes',
    initialState,
    selectors: {
        selectRecipes: (state) => state.recipes,
        selectIsLoading: (state) => state.isLoading,
        selectError: (state) => state.error,
    },
    reducers: {},
    extraReducers: (builder) => {
        builder
            // fetch
            .addCase(fetchRecipes.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchRecipes.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = action.payload;
            })
            .addCase(fetchRecipes.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // delete
            .addCase(deleteRecipe.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(deleteRecipe.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = state.recipes.filter(
                    (recipe) => recipe.id !== action.payload
                );
            })
            .addCase(deleteRecipe.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // add
            .addCase(addRecipe.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(addRecipe.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = action.payload;
            })
            .addCase(addRecipe.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // update
            .addCase(updateRecipe.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updateRecipe.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = action.payload;
            })
            .addCase(updateRecipe.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            });
    },
});

export const { selectRecipes, selectIsLoading, selectError } =
    RecipesSlice.selectors;

export default RecipesSlice.reducer;
