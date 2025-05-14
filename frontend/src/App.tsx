import React, { useEffect, useRef, useState } from 'react';
import './App.css';
import { useDispatch, useSelector } from './store/store';
import {
    deleteRecipe,
    updateRecipe,
    fetchRecipes,
    selectRecipes,
} from './store/recipes.slice';
import { RECIPES } from './mocks';

function App() {
    const dispatch = useDispatch();
    // const recipes = useSelector(selectRecipes);
    const recipes = RECIPES;
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingRecipeId, setEditingRecipeId] = useState<string | null>(null);

    const titleRef = useRef<HTMLInputElement>(null);
    const descRef = useRef<HTMLTextAreaElement>(null);
    const imageRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        dispatch(fetchRecipes());
    }, [dispatch]);

    const openEditModal = (id: string) => {
        setEditingRecipeId(id);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setEditingRecipeId(null);
    };

    const handleDelete = (id: string) => {
        dispatch(deleteRecipe(id));
    };

    const handleSave = () => {
        if (!editingRecipeId) return;

        const dto = {
            title: titleRef.current?.value,
            description: descRef.current?.value,
        };

        const imageFile = imageRef.current?.files?.[0];

        dispatch(
            updateRecipe({
                id: editingRecipeId,
                dto,
                imageFile,
            })
        );

        closeModal();
    };

    const editingRecipe = recipes.find((r) => r.id === editingRecipeId);

    return (
        <main className='App'>
          <h1>Список рецептов</h1>
            <ul className='recipe-list'>
                {recipes.map((recipe) => (
                    <li key={recipe.id} className='recipe-item'>
                        <img
                            src={
                                recipe.mainPictureUrl ||
                                'https://via.placeholder.com/100'
                            }
                            alt={recipe.title}
                            className='recipe-image'
                        />
                        <div className='recipe-info'>
                            <h3 className='recipe-title'>{recipe.title}</h3>
                            <p className='recipe-description'>
                                {recipe.description}
                            </p>
                        </div>
                        <div className='recipe-actions'>
                            <button onClick={() => openEditModal(recipe.id)}>
                                ✏️
                            </button>
                            <button onClick={() => handleDelete(recipe.id)}>
                                🗑️
                            </button>
                        </div>
                    </li>
                ))}
            </ul>

            {isModalOpen && editingRecipe && (
                <div className='modal-backdrop'>
                    <div className='modal'>
                        <button className='modal-close' onClick={closeModal}>
                            ✖
                        </button>
                        <h2>Редактировать рецепт</h2>
                        <input
                            ref={titleRef}
                            defaultValue={editingRecipe.title}
                            placeholder='Название'
                        />
                        <textarea
                            ref={descRef}
                            defaultValue={editingRecipe.description}
                            placeholder='Описание'
                        />
                        <input ref={imageRef} type='file' accept='image/*' />
                        <div className='modal-actions'>
                            <button onClick={handleSave}>💾 Сохранить</button>
                            <button onClick={closeModal}>❌ Отмена</button>
                        </div>
                    </div>
                </div>
            )}
        </main>
    );
}

export default App;
