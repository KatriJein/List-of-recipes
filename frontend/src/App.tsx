import { useEffect, useRef, useState } from 'react';
import './App.css';
import { useDispatch, useSelector } from './store/store';
import {
    deleteRecipe,
    updateRecipe,
    fetchRecipes,
    selectRecipes,
    addRecipe,
    selectIsLoading,
} from './store/recipes.slice';

function App() {
    const dispatch = useDispatch();
    const recipes = useSelector(selectRecipes);
    const isLoading = useSelector(selectIsLoading);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingRecipeId, setEditingRecipeId] = useState<string | null>(null);
    const [imagePreview, setImagePreview] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);

    const titleRef = useRef<HTMLInputElement>(null);
    const descRef = useRef<HTMLTextAreaElement>(null);
    const imageRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        dispatch(fetchRecipes());
    }, [dispatch]);

    const openEditModal = (id: string | null) => {
        setEditingRecipeId(id);
        const recipe = recipes.find((r) => r.id === id);
        setImagePreview(recipe?.presignedPictureUrl || null);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setEditingRecipeId(null);
        setImagePreview(null);
    };

    const handleDelete = (id: string) => {
        dispatch(deleteRecipe(id));
    };

    const handleImageChange = () => {
        const file = imageRef.current?.files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result as string);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleSave = () => {
        setIsSaving(true);

        const dto = {
            title: titleRef.current?.value || '',
            description: descRef.current?.value || '',
        };

        const imageFile = imageRef.current?.files?.[0];

        if (editingRecipeId) {
            dispatch(updateRecipe({ id: editingRecipeId, dto, imageFile }))
                .unwrap()
                .catch((e) => console.error('Ошибка при обновлении', e))
                .finally(() => {
                    setIsSaving(false);
                    closeModal();
                });
        } else {
            dispatch(addRecipe({ dto, imageFile }))
                .unwrap()

                .catch((e) => console.error('Ошибка при добавлении', e))
                .finally(() => {
                    setIsSaving(false);
                    closeModal();
                });
        }
    };

    const editingRecipe = editingRecipeId
        ? recipes.find((r) => r.id === editingRecipeId)
        : null;

    return (
        <main className='App'>
            <div className='header'>
                <h1>Список рецептов</h1>
                <button onClick={() => openEditModal(null)}>
                    Добавить рецепт
                </button>
            </div>
            {isLoading ? (
                <div className='loading'>Загрузка...</div>
            ) : (
                <ul className='recipe-list'>
                    {recipes.map((recipe) => (
                        <li key={recipe.id} className='recipe-item'>
                            <img
                                src={
                                    recipe.presignedPictureUrl ||
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
                                <button
                                    onClick={() => openEditModal(recipe.id)}
                                >
                                    ✏️
                                </button>
                                <button onClick={() => handleDelete(recipe.id)}>
                                    🗑️
                                </button>
                            </div>
                        </li>
                    ))}
                </ul>
            )}

            {isModalOpen && (
                <div className='modal-backdrop'>
                    <div className='modal'>
                        <button className='modal-close' onClick={closeModal}>
                            ✖
                        </button>
                        <h2>
                            {editingRecipeId ? 'Редактировать' : 'Добавить'}{' '}
                            рецепт
                        </h2>
                        <input
                            ref={titleRef}
                            defaultValue={editingRecipe?.title || ''}
                            placeholder='Название'
                        />
                        <textarea
                            ref={descRef}
                            defaultValue={editingRecipe?.description || ''}
                            placeholder='Описание'
                        />
                        <input
                            ref={imageRef}
                            type='file'
                            accept='image/*'
                            onChange={handleImageChange}
                        />
                        {imagePreview && (
                            <img
                                src={imagePreview}
                                alt='Превью'
                                style={{
                                    maxWidth: '100%',
                                    marginTop: '10px',
                                }}
                            />
                        )}
                        <div className='modal-actions'>
                            <button onClick={handleSave}>
                                {isSaving ? 'Сохраняем...' : '💾 Сохранить'}
                            </button>
                            <button onClick={closeModal}>❌ Отмена</button>
                        </div>
                    </div>
                </div>
            )}
        </main>
    );
}

export default App;
