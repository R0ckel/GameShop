import React, {useEffect, useState} from 'react';
import {KeyValueTable} from '../helpers/keyValueTable';
import {useParams, useNavigate} from "react-router-dom";
import styles from '../../css/app.module.css';
import {GamesService} from "../../services/domain/gamesService";
import {Button, Pagination,} from "antd";
import {ErrorPage} from "../responses/errorPage";
import defaultGamePicture from "../../image/game-icon.png";
import Image from "../helpers/image";
import {gameImagesApiUrl} from "../../variables/connectionVariables";
import {BasketService} from "../../services/domain/basketService";
import {useDispatch, useSelector} from "react-redux";
import {basketUpdated} from "../../context/store";
import {CommentForm} from "./commentForm";
import {CommentService} from "../../services/domain/commentService";
import {Comment} from "./comment";

export default function GameDetails () {
  const {id} = useParams()
  const navigate = useNavigate();
  const [gameData, setGameData] = useState(null)
  const {basketItems} = useSelector(state => state.basketData)
  const {isLoggedIn} = useSelector(state => state.userData)
  const dispatch = useDispatch()
  const [comments, setComments] = useState([]);
  const [commentResponse, setCommentResponse] = useState({});
  const [commentFilters, setCommentFilters] = useState({});
  const [lastCommentUpdate, setLastCommentUpdate] = useState(Date.now())

  useEffect(() => {
    async function fetchData(){
      setGameData((await GamesService.getById(id)).values[0])
    }
    fetchData()
  }, [id])

  useEffect(() => {
    async function fetchComments(){
      const response = await CommentService.getByGameId(id, commentFilters);
      setCommentResponse(response);
      if (response.success) setComments(response.values)
    }
    fetchComments()
  }, [commentFilters, id, lastCommentUpdate])

  if (gameData === null) {
    return <ErrorPage code={404}></ErrorPage>
  }

  const tableItem = {
    company: gameData.companyName,
    genres: gameData.genres.map(x => x.name).join(', '),
    price: gameData.price + '$'
  }

  async function addToBasket(){
    await BasketService.addItem(gameData.id)
    dispatch(basketUpdated())
  }
  async function removeFromBasket(){
    await BasketService.removeItem(gameData.id)
    dispatch(basketUpdated())
  }

  const handleFilterChange = async (key, value) => {
    setCommentFilters(prevFilters => ({ ...prevFilters, [key]: value }));
  };

  if (comments.length === 0 && commentFilters?.page && commentFilters?.page !== 1){
    handleFilterChange('page', 1);
  }

  function updateComments(){
    setLastCommentUpdate(Date.now())
  }

  return (
    <main>
      <div style={{display: "block"}}>
        <button className={`${styles.btn} ${styles.white}`} onClick={() => navigate(-1)}>
          &larr; Go back
        </button>
      </div>

      <div className={`${styles.centeredInfoBlock}`}>
        <div style={{
          display: "flex",
          justifyContent: "space-evenly"
        }}>
          <Image
            key={`${gameImagesApiUrl}/${id}_gameImage`}
            imageClassName={`${styles.gamePictureSize} ${styles.smoothBorder}`}
            containerClassName={`${styles.backgroundHighlighted} ${styles.smoothBorder} ${styles.centerInnerVertical}`}
            src={`${gameImagesApiUrl}/${id}`}
            defaultImage={defaultGamePicture}
          />
          <div style={{marginLeft: '30px'}}>
            <h1>{gameData?.name}</h1>
            <KeyValueTable item = {tableItem} />
            <span>
              <h3>Description</h3>
              <span>{gameData.description}</span>
            </span>
          </div>
        </div>

        { isLoggedIn ?
          <>
            { basketItems?.filter(x => x.gameId === gameData.id).length > 0 ?
              <Button danger={true} type={"primary"} className={styles.wideButton} onClick={removeFromBasket}>
                Remove from basket
              </Button> :
              <Button type='primary' className={styles.wideButton} onClick={addToBasket}>
                Add to basket
              </Button>
            }
            <CommentForm gameId={gameData.id} updateCommentList={updateComments}/>
          </>
          : null
        }

      </div>

      {comments.length > 0 ?
        <>
          <h1 className={styles.centered} style={{marginTop: '50px'}}>Comments</h1>
          <Pagination
            current={commentResponse?.pageNumber}
            total={!isNaN(commentResponse?.pageCount * commentResponse?.pageSize)
              ? commentResponse?.pageCount * commentResponse?.pageSize
              : 1 }
            pageSize={commentResponse?.pageSize ?? 10}
            onChange={value => handleFilterChange('page', value)}
            onShowSizeChange={(current, size) => handleFilterChange('pageSize', size)}
            showSizeChanger
            pageSizeOptions={['2', '5', '10', '20', '50']}
            style={{backgroundColor: 'rgba(255, 255, 255, 0.3)'}}
            className={`${styles.smoothBorder} ${styles.centered}`}
          />

          <div>
            {comments.map(comment => {
              return <Comment
                key={`comment_${comment?.userId}_${comment?.created}`}
                comment={comment}
                updateCommentList={updateComments}
              />
            })}
          </div>
        </>
        : null
      }
    </main>
  )
}